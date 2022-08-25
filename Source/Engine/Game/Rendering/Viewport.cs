using System;
using Engine.Frontend;
using Engine.GPU;
using Engine.World;
using Vortice.DXGI;

namespace Engine.Rendering
{
	[StructLayout(LayoutKind.Sequential)]
	public struct ViewConstants
	{
		public Matrix4 View;
		public Matrix4 Projection;
	}

	/// <summary>
	/// Contains the game logic for a UI viewport
	/// </summary>
	public unsafe class Viewport : IDisposable
	{
		public static List<Viewport> All { get; } = new();

		// Work camera settings.
		public CameraActor WorkCamera = new CameraActor();

		// Constant buffers
		public GraphicsBuffer<ViewConstants> ViewConstantsBuffer = new(1, GraphicsBuffer.ConstantAlignment);

		// Helpers
		public ViewportHost Host { get; }
		public Vector2i Size => Host.Swapchain.RT.Size;

		// Render targets and buffers
		public Texture ColorTarget;
		public Texture DepthBuffer;

		// Rendering command list.
		public CommandList CommandList = new();

		/// <summary>
		/// Constructs a viewport from a given UI host
		/// </summary>
		public Viewport(ViewportHost host)
		{
			Host = host;
			All.Add(this);
			
			host.Swapchain.OnResize += Resize;

			// Create RTs and RT-sized buffers.
			ColorTarget = new Texture(Size.X, Size.Y, 1, Format.R8G8B8A8_UNorm);
			DepthBuffer = new Texture(Size.X, Size.Y, 1, Format.R32_Typeless, dsFormat: Format.D32_Float, srFormat: Format.R32_Float);
		}

		public void UpdateView()
		{
			// Calculate view/projection matrices.
			Matrix4 view = Matrix4.CreateTransform(WorkCamera.Position, WorkCamera.Rotation, Vector3.One).Inverted();
			Matrix4 projection = Matrix4.CreatePerspectiveReversed(WorkCamera.CalcFOV(), Size.X / (float)Size.Y, 0.01f);

			ViewConstantsBuffer.SetData(0, new ViewConstants()
			{
				View = view,
				Projection = projection,
			});
		}

		/// <summary>
		/// Resizes the viewport and it's RTs
		/// </summary>
		private void Resize(Vector2i size)
		{
			ColorTarget.Resize(size.X, size.Y);
			DepthBuffer.Resize(size.X, size.Y);
		}

		public void Dispose()
		{
			ColorTarget.Dispose();
			DepthBuffer.Dispose();
			All.Remove(this);
		}
	}
}
