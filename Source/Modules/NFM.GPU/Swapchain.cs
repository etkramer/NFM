using System;
using SharpGen.Runtime;
using Vortice.Direct3D12;
using Vortice.DXGI;

namespace NFM.GPU
{
	public class Swapchain : IDisposable
	{
		public event Action<Vector2i> OnResize = delegate{};

		public int PresentInterval { get; private set; }
		public Texture RT => backbuffers[swapchain.CurrentBackBufferIndex];
		public Vector2i Size { get; private set; }

		private IDXGISwapChain4 swapchain;
		private Texture[] backbuffers;

		private SwapChainFlags flags;

		public Swapchain(IntPtr hwnd, int presentInterval = 0)
		{
			PresentInterval = presentInterval;

			flags = SwapChainFlags.None;
			flags |= SwapChainFlags.FrameLatencyWaitableObject;
			if (Graphics.SupportsTearing)
			{
				flags |= SwapChainFlags.AllowTearing;
			}

			// Describe swapchain.
			SwapChainDescription1 swapchainDesc = new()
			{
				BufferCount = Graphics.RenderLatency,
				Width = 0,
				Height = 0,
				Format = Graphics.RTFormat,
				BufferUsage = Usage.RenderTargetOutput,
				SwapEffect = SwapEffect.FlipDiscard,
				SampleDescription = new SampleDescription(1, 0),
				Flags = flags,
			};

			// Create swapchain.
			swapchain = Graphics.DXGIFactory.CreateSwapChainForHwnd(Graphics.GraphicsQueue, hwnd, swapchainDesc).QueryInterface<IDXGISwapChain4>();

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

		private void CreateRTs()
		{
            // Create RTs for each backbuffer.
            backbuffers = new Texture[Graphics.RenderLatency];
            for (int i = 0; i < Graphics.RenderLatency; i++)
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

			Debug.Assert(swapchain.Present(PresentInterval, (PresentInterval == 0 && Graphics.SupportsTearing) ? PresentFlags.AllowTearing : PresentFlags.None).Success, "Swapchain present failed");
		}

		public void Resize(Vector2i size)
		{
			Size = size;

			// Don't resize to an invalid resolution.
			if (size.X < 2 || size.Y < 2)
			{
				return;
			}

			// Wait for the GPU to finish up with any commands that might be using this swapchain.
			Graphics.Flush();

			// Dispose existing render targets.
			for (int i = 0; i < Graphics.RenderLatency; i++)
			{
				backbuffers[i].Dispose();
			}

			// Resize swapchain.
			swapchain.ResizeBuffers(Graphics.RenderLatency, size.X, size.Y, Format.Unknown, flags);

			// Recreate render targets.
			CreateRTs();

			// Invoke resize callback.
			OnResize.Invoke(Size);
		}
	}
}
