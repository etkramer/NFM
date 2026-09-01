using System;
using System.Diagnostics.CodeAnalysis;
using SharpGen.Runtime;
using Vortice.Direct3D12;
using Vortice.DXGI;

namespace NFM.GPU;

public class Swapchain : IDisposable
{
	public event Action<Vector2i> OnResize = delegate{};

	public int PresentInterval { get; private set; }
	public Texture RT => backbuffers[swapchain.CurrentBackBufferIndex];
	public Vector2i Size { get; private set; }

	/// <summary>
	/// The underlying IDXGISwapChain, for use as the content of a composition visual.
	/// </summary>
	public IntPtr NativePointer => swapchain.NativePointer;

	private IDXGISwapChain4 swapchain;
	private Texture[] backbuffers;

	private readonly SwapChainFlags flags;

	public Swapchain(IntPtr hwnd, int presentInterval = 0)
	{
		PresentInterval = presentInterval;

		flags = SwapChainFlags.None;
		flags |= SwapChainFlags.FrameLatencyWaitableObject;
		if (D3DContext.SupportsTearing)
		{
			flags |= SwapChainFlags.AllowTearing;
		}

		SwapChainDescription1 swapchainDesc = Describe(0, 0, AlphaMode.Unspecified);
		Init(Guard.NotNull(D3DContext.DXGIFactory).CreateSwapChainForHwnd(D3DContext.GraphicsQueue, hwnd, swapchainDesc));
	}

	/// <summary>
	/// Creates a swapchain for use as the content of a composition visual. Unlike the hwnd variant, this
	/// can't infer its size from a window, and DWM composites the result - so tearing isn't available.
	/// </summary>
	public Swapchain(Vector2i size, int presentInterval = 0)
	{
		PresentInterval = presentInterval;

		flags = SwapChainFlags.None;
		flags |= SwapChainFlags.FrameLatencyWaitableObject;

		SwapChainDescription1 swapchainDesc = Describe(MathHelper.Max(size.X, 1), MathHelper.Max(size.Y, 1), AlphaMode.Ignore);
		Init(Guard.NotNull(D3DContext.DXGIFactory).CreateSwapChainForComposition(D3DContext.GraphicsQueue, swapchainDesc));
	}

	private SwapChainDescription1 Describe(int width, int height, AlphaMode alphaMode) => new()
	{
		BufferCount = D3DContext.RenderLatency,
		Width = width,
		Height = height,
		Format = D3DContext.RTFormat,
		BufferUsage = Usage.RenderTargetOutput,
		SwapEffect = SwapEffect.FlipDiscard,
		AlphaMode = alphaMode,
		Scaling = Scaling.Stretch,
		SampleDescription = new SampleDescription(1, 0),
		Flags = flags,
	};

	[MemberNotNull(nameof(backbuffers), nameof(swapchain))]
	private void Init(IDXGISwapChain1 created)
	{
		swapchain = created.QueryInterface<IDXGISwapChain4>();
		created.Release();

		// Update size to match actual used by swapchain.
		var swapchainSize = swapchain.SourceSize;
		Size = new(swapchainSize.Width, swapchainSize.Height);

		// Create render targets.
		CreateRTs();

		// Present, so we can get a black screen before it has time to blind us.
		Present();
	}

	public void Dispose()
	{
		foreach (Texture backbuffer in backbuffers)
		{
			backbuffer.Dispose();
		}

		swapchain.Release();
	}

    [MemberNotNull(nameof(backbuffers))]
	private void CreateRTs()
	{
        // Create RTs for each backbuffer.
        backbuffers = new Texture[D3DContext.RenderLatency];
        for (int i = 0; i < D3DContext.RenderLatency; i++)
        {
			// Create backbuffer RT.
			backbuffers[i] = new Texture(swapchain.GetBuffer<ID3D12Resource>(i), Size.X, Size.Y);
			backbuffers[i].State = ResourceStates.Present;
        }
	}

	/// <summary>
	/// Adds a present operation to the command queue
	/// </summary>
	public void Present()
	{
		if (RT.State != ResourceStates.Present)
		{
			throw new InvalidOperationException("Resource is in wrong state for present!");
		}

		bool allowTearing = PresentInterval == 0 && flags.HasFlag(SwapChainFlags.AllowTearing);
		Guard.Require(swapchain.Present(PresentInterval, allowTearing ? PresentFlags.AllowTearing : PresentFlags.None).Success, "Swapchain present failed");
	}

	public void Resize(Vector2i size)
	{
		// Don't resize to an invalid resolution.
		if (size.X < 2 || size.Y < 2)
		{
			return;
		}

		Size = size;

		// Wait for the GPU to finish up with any commands that might be using this swapchain.
		D3DContext.Flush();

		// Dispose existing render targets.
		for (int i = 0; i < D3DContext.RenderLatency; i++)
		{
			backbuffers[i].Dispose(true);
		}

		// Resize swapchain.
		swapchain.ResizeBuffers(D3DContext.RenderLatency, size.X, size.Y, Format.Unknown, flags);

		// Recreate render targets.
		CreateRTs();

		// Invoke resize callback.
		OnResize.Invoke(Size);
	}
}
