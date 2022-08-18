global using Engine.Core;
global using Engine.Mathematics;
using Vortice.DXGI;
using Vortice.Direct3D12;
using Vortice.Direct3D12.Debug;
using System.Runtime.InteropServices;
using Feature = Vortice.Direct3D12.Feature;

namespace Engine.GPU
{
	public static class GPUContext
	{
		public static int RenderLatency = 2;
		public static ulong FrameCount = 0;
		public static int FrameIndex => (int)(FrameCount % (ulong)RenderLatency);

		internal static bool SupportsTearing = false;

		internal static Format RTFormat = Format.R8G8B8A8_UNorm;
		internal static Format DSFormat = Format.D32_Float;

		internal static ID3D12Device6 Device;
		internal static IDXGIFactory6 DXGIFactory;
		internal static ID3D12CommandQueue GraphicsQueue;

		private static ID3D12Fence frameFence;
		private static ID3D12Fence flushFence;
		private static AutoResetEvent frameFenceEvent;

		private static unsafe void DebugCallback(MessageCategory category, MessageSeverity severity, MessageId id, void* description, void* context)
		{
			string message = Marshal.PtrToStringAnsi((IntPtr)description);

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
			if (Debug.IsDebugBuild && D3D12.D3D12GetDebugInterface(out ID3D12Debug3 debug).Success)
			{
				debug.EnableDebugLayer();
				debug.Dispose();
			}

			// Create DXGI factory.
			DXGI.CreateDXGIFactory2(Debug.IsDebugMode, out DXGIFactory);

			// Create D3D12 device.
			if (!TryCreateDevice(out Device))
			{
				throw new NotSupportedException("GPU does not support Direct3D 12 Ultimate.");
			}

			// Do some extra debug setup.
			if (Debug.IsDebugBuild)
			{
				ID3D12InfoQueue1 infoQueue = Device.QueryInterfaceOrNull<ID3D12InfoQueue1>();

				// RenderDoc makes the query fail for whatever reason.
				if (infoQueue != null)
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

		private static bool TryCreateDevice(out ID3D12Device6 device)
		{
			// Find the ideal hardware adapter.
			for (int i = 0; DXGIFactory.EnumAdapterByGpuPreference(i, GpuPreference.HighPerformance, out IDXGIAdapter2 adapter).Success; i++)
			{
				// Create D3D12 device with Feature Level 12.2 (Ultimate).
				if (D3D12.D3D12CreateDevice(adapter, Vortice.Direct3D.FeatureLevel.Level_12_2, out device).Success)
				{
					adapter.Dispose();
					return true;
				}

				adapter.Dispose();
			}

			device = null;
			return false;
		}

		internal static void WaitIdle()
		{
			ulong fenceValue = flushFence.CompletedValue;
			GraphicsQueue.Signal(flushFence, fenceValue + 1);
			flushFence.SetEventOnCompletion(fenceValue + 1, frameFenceEvent);
			frameFenceEvent.WaitOne();
		}

		internal static void WaitFrame()
		{
			GraphicsQueue.Signal(frameFence, ++FrameCount);

			// Is the CPU more than RenderLatency frames ahead of the GPU?
			if (FrameCount - frameFence.CompletedValue >= (ulong)RenderLatency)
			{
				// Wait for the GPU to catch up.
				frameFence.SetEventOnCompletion(FrameCount, frameFenceEvent);
				frameFenceEvent.WaitOne();
			}
		}

		public static IntPtr GetDevicePointer()
		{
			return Device.NativePointer;
		}
	}
}