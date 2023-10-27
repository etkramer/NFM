global using NFM.Common;
global using NFM.Mathematics;
using Vortice.DXGI;
using Vortice.Direct3D12;
using Vortice.Direct3D12.Debug;
using System.Runtime.InteropServices;
using System.Diagnostics.CodeAnalysis;

namespace NFM.GPU;

public static class D3DContext
{
	public static int RenderLatency = 2;
	public static int FrameIndex => (int)(Metrics.FrameCount % (ulong)RenderLatency);

	public static event Action OnFrameStart = delegate {};

	internal static bool SupportsTearing = false;

	internal static Format RTFormat = Format.R8G8B8A8_UNorm;
	internal static Format DSFormat = Format.D32_Float;

	internal static ID3D12Device6? Device;
	internal static IDXGIFactory6? DXGIFactory;
	internal static ID3D12CommandQueue? GraphicsQueue;

	private static ID3D12Fence? frameFence;
	private static ID3D12Fence? flushFence;
	private static AutoResetEvent? frameFenceEvent;

	private static unsafe void DebugCallback(MessageCategory category, MessageSeverity severity, MessageId id, void* description, void* context)
	{
		string message = Guard.NotNull(Marshal.PtrToStringAnsi((IntPtr)description));

		if (severity == MessageSeverity.Corruption || severity == MessageSeverity.Error)
		{
			Debug.LogError(message);
			throw new Exception(message);
		}
		else if (severity == MessageSeverity.Warning)
		{
			Debug.LogWarning(message);
		}
		else
		{
			Debug.Log(message);
		}
	}

	public static void Init(int renderLatency = 2)
	{
		// Clamp RenderLatency to a minimum of 2 (double buffered)
		RenderLatency = MathHelper.Max(renderLatency, 2);

		// Enable debug layer in debug builds.
		if (Debug.IsDebugBuild && D3D12.D3D12GetDebugInterface(out ID3D12Debug5? debug).Success)
		{
            Guard.NotNull(debug);

			debug.EnableDebugLayer();
			debug.SetEnableAutoName(true);
			debug.Dispose();
		}

		// Create DXGI factory.
		DXGI.CreateDXGIFactory2(Debug.IsDebugMode, out DXGIFactory);
        Guard.NotNull(DXGIFactory);

		// Create D3D12 device.
		if (!TryCreateDevice(out Device))
		{
			throw new NotSupportedException("GPU does not support Direct3D 12 Ultimate.");
		}

		// Do some extra debug setup.
		if (Debug.IsDebugBuild)
		{
			var infoQueue = Device.QueryInterfaceOrNull<ID3D12InfoQueue1>();

			// RenderDoc makes the query fail for whatever reason.
			if (infoQueue is not null)
			{
				unsafe
				{
					// Setup debug callbacks.
					int cookie = 0;
					delegate*<MessageCategory, MessageSeverity, MessageId, void*, void*, void> callback = &DebugCallback;
					infoQueue.RegisterMessageCallback(new(callback), MessageCallbackFlags.None, IntPtr.Zero, ref cookie);
				}
			}
		}

		// Check feature support.
		{
			SupportsTearing = DXGIFactory.PresentAllowTearing;
		}

		// Create graphics command queue.
		GraphicsQueue = Device.CreateCommandQueue(CommandListType.Direct);
		GraphicsQueue.Name = "Graphics Queue";

		// Create frame fences.
		frameFence = Device.CreateFence(0);
		flushFence = Device.CreateFence(0);
		frameFenceEvent = new AutoResetEvent(false);
	}

	private static bool TryCreateDevice([NotNullWhen(true)] out ID3D12Device6? device)
	{
        Guard.NotNull(DXGIFactory);

		// Find the ideal hardware adapter.
		for (int i = 0; DXGIFactory.EnumAdapterByGpuPreference(i, GpuPreference.HighPerformance, out IDXGIAdapter2? adapter).Success; i++)
		{
			// Create D3D12 device with Feature Level 12.2 (Ultimate).
			if (D3D12.D3D12CreateDevice(adapter, Vortice.Direct3D.FeatureLevel.Level_12_2, out device).Success)
			{
                Guard.NotNull(device);

				adapter?.Dispose();
				return true;
			}

			adapter?.Dispose();
		}

		device = null;
		return false;
	}

	public static bool WaitFrame()
	{
        Guard.NotNull(GraphicsQueue);
        Guard.NotNull(frameFence);
        Guard.NotNull(frameFenceEvent);


		GraphicsQueue.Signal(frameFence, Metrics.FrameCount);
		ulong frameCountGPU = frameFence.CompletedValue;

		// If we are more than RenderLatency frames ahead, wait for the GPU to catch up.
		bool result = false;
		if ((Metrics.FrameCount - frameCountGPU) >= (ulong)RenderLatency)
		{
			frameFence.SetEventOnCompletion(frameCountGPU + 1, frameFenceEvent);
			frameFenceEvent.WaitOne();

			result = true;
		}

		// Release pending objects
		while (releaseQueue.Count > 0)
		{
			var first = releaseQueue.Peek();
			if (first.Item1 >= frameFence.CompletedValue)
			{
				first.Item2.Release();
				releaseQueue.Dequeue();
			}
			else
			{
				// releaseQueue is always in chronological order.
				// If this object isn't ready to be released, neither are any others.
				break;
			}
		}

		// Let systems know it's the start of a new frame.
		Metrics.BeginFrame();
		OnFrameStart.Invoke();

		return result;
	}

	public static void Flush()
	{
		ulong fenceValue = Guard.NotNull(flushFence).CompletedValue;

	    Guard.NotNull(GraphicsQueue).Signal(flushFence, fenceValue + 1);
		flushFence.SetEventOnCompletion(fenceValue + 1);
	}

	// Always in order
	private static Queue<(ulong, ID3D12Object)> releaseQueue = new();

	/// <summary>
	/// Releases the specified object once it is no longer in use by the GPU
	/// </summary>
	public static void SafeRelease<T>(this T subject) where T : ID3D12Resource
	{
		releaseQueue.Enqueue((Metrics.FrameCount, subject));
	}
}