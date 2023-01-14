using System;
using System.Buffers;
using NFM.GPU;
using NFM.World;
using Vortice.DXGI;

namespace NFM.Rendering
{
	[StructLayout(LayoutKind.Sequential)]
	public struct ViewConstants
	{
		public Matrix4 WorldToView;
		public Matrix4 ViewToWorld;

		public Matrix4 ViewToClip;
		public Matrix4 ClipToView;

		public Vector2 ViewportSize;
	}

	public class RenderTarget : IDisposable
	{
		#region Cache
		private static List<RenderTarget> rtCache = new();

		static RenderTarget()
		{
			Engine.OnTick += (t) =>
			{
				for (int i = rtCache.Count - 1; i >= 0; i--)
				{
					// Was this RT used in the last frame?
					if (rtCache[i].lastFrame != Metrics.FrameCount - 1)
					{
						// If not, dispose it.
						rtCache[i].Dispose();
						rtCache.RemoveAt(i);
					}
				}
			};
		}

		public static RenderTarget Get(Vector2i size)
		{
			// Try to find an RT that matches the requested size and isn't already being used this frame.
			var rt = rtCache.FirstOrDefault(o => o.Size == size && o.lastFrame != Metrics.FrameCount);

			// Create a new RT if necessary.
			if (rt == null)
			{
				rt = new RenderTarget(size);
				rtCache.Add(rt);
			}

			// Update it's last frame and return it.
			rt.lastFrame = Metrics.FrameCount;
			return rt;
		}

		// The last frame where this RT was actually used.
		private ulong lastFrame = 0;

		#endregion

		public Vector2i Size;

		// Command list and constants
		public CommandList CommandList = new();
		public GraphicsBuffer<ViewConstants> ViewCB = new(1, GraphicsBuffer.ConstantAlignment);

		// Textures/buffers
		public Texture ColorTarget;
		public Texture DepthBuffer;

		public RenderTarget(Vector2i size)
		{
			Size = size;
			CommandList.Name = $"RT List (size {size})";

			// Create RTs and RT-sized buffers.
			ColorTarget = new Texture(Size.X, Size.Y, 1, Format.R8G8B8A8_UNorm, samples: 1);
			DepthBuffer = new Texture(Size.X, Size.Y, 1, Format.R32_Typeless, dsFormat: Format.D32_Float, srFormat: Format.R32_Float, samples: 1);
		}

		public void Dispose()
		{
			ColorTarget.Dispose();
			DepthBuffer.Dispose();
		}

		public Matrix4 ViewMatrix { get; private set; }
		public Matrix4 ProjectionMatrix { get; private set; }

		public void UpdateView(CameraNode camera)
		{
			// Calculate view/projection matrices.
			ViewMatrix = camera.WorldTransform.Inverse();
			ProjectionMatrix = Matrix4.CreatePerspectiveReversed(camera.FOV, Size.X / (float)Size.Y, 0.01f);

			// Apply Z-up projection.
			ProjectionMatrix = Matrix4.CreateRotation(new(-90, 180, 0)) * ProjectionMatrix;

			// Upload to constant buffer.
			CommandList.UploadBuffer(ViewCB, new ViewConstants()
			{
				WorldToView = ViewMatrix,
				ViewToWorld = ViewMatrix.Inverse(),
				ViewToClip = ProjectionMatrix,
				ClipToView = ProjectionMatrix.Inverse(),
				ViewportSize = Size,
			});
		}
	}
}
