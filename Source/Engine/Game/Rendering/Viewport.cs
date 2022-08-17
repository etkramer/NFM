using System;
using Engine.Frontend;
using Engine.GPU;
using Vortice.DXGI;

namespace Engine.Rendering
{
	/// <summary>
	/// Contains the game logic for a UI viewport
	/// </summary>
	public unsafe class Viewport : IDisposable
	{
		public static List<Viewport> All { get; } = new();

		// Constant buffers
		public GraphicsBuffer<Matrix4> ViewBuffer = new(1);

		// Helpers
		public ViewportHost Host { get; }
		public Vector2i Size => Host.Swapchain.RT.Size;

		// Render targets
		public Texture ColorTarget;
		public Texture DepthBuffer;

		/// <summary>
		/// Constructs a viewport from a given UI host
		/// </summary>
		public Viewport(ViewportHost host)
		{
			Host = host;
			All.Add(this);
			
			host.OnResize += Resize;
			ColorTarget = new Texture((ulong)Size.X, (ulong)Size.Y, 1, Format.R8G8B8A8_UNorm, default(Color));
			DepthBuffer = new Texture((ulong)Size.X, (ulong)Size.Y, 1, Format.D32_Float, new(0f));
		}

		public void UpdateView()
		{
			float aspect = (Size.X / (float)Size.Y);
			Matrix4 projection = Matrix4.CreatePerspectiveReversed(60f, aspect, 0.01f);

			ViewBuffer.SetData(0, projection);
		}

		/// <summary>
		/// Resizes the viewport and it's RTs
		/// </summary>
		private void Resize(Vector2i size)
		{
			ColorTarget.Resize((ulong)size.X, (ulong)size.Y);
			DepthBuffer.Resize((ulong)size.X, (ulong)size.Y);
		}

		public void Dispose()
		{
			ColorTarget.Dispose();
			DepthBuffer.Dispose();
			All.Remove(this);
		}
	}
}
