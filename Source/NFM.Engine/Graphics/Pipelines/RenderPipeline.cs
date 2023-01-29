using System;
using NFM.GPU;
using NFM.World;

namespace NFM.Graphics;

[StructLayout(LayoutKind.Sequential)]
public struct ViewConstants
{
	public Matrix4 WorldToView;
	public Matrix4 ViewToWorld;

	public Matrix4 ViewToClip;
	public Matrix4 ClipToView;

	public Vector3 EyePosition;
}

/// <summary>
/// Describes a series of steps for rendering an image.
/// Should contain any size-dependent resources - gbuffers, post-processing targets, etc.
/// One RenderPipeline instance may be recycled for multiple render targets, but only of the same size/resolution.
/// </summary>
abstract class RenderPipeline<TSelf> : IDisposable where TSelf : RenderPipeline<TSelf>, new()
{
	#region Cache

	static RenderPipeline()
	{
		Engine.OnTick += (t) =>
		{
			for (int i = rpCache.Count - 1; i >= 0; i--)
			{
				// Was this RT used in the last frame?
				if (rpCache[i].lastFrame != Metrics.FrameCount - 1)
				{
					// If not, dispose it.
					rpCache[i].Dispose();
					rpCache.RemoveAt(i);
				}
			}
		};
	}

	private static List<TSelf> rpCache = new();
	public static TSelf Get(Texture rt)
	{
		// Try to find an RT that matches the requested size and isn't already being used this frame.
		var rp = rpCache.FirstOrDefault(o => o.Size == rt.Size && o.lastFrame != Metrics.FrameCount);

		// Create a new RT if necessary.
		if (rp == null)
		{
			rp = new TSelf()
			{
				Size = rt.Size
			};

			rp.Init(rt.Size);
			rpCache.Add(rp);
		}

		// Update it's last frame and return it.
		rp.lastFrame = Metrics.FrameCount;
		return rp;
	}

	// The last frame where this RP was actually used.
	private ulong lastFrame = 0;

	#endregion

	public GraphicsBuffer<ViewConstants> ViewCB { get; } = new(1, GraphicsBuffer.ConstantAlignment);

	public CommandList List { get; } = new CommandList();	
	protected Vector2i Size { get; private set; } = default;

	private static List<CameraStep<TSelf>> renderSteps= new();
	protected static void AddStep<T>() where T : CameraStep<TSelf>, new()
	{
		if (!renderSteps.Any(o => o.GetType() == typeof(T)))
		{
			var step = new T();
			renderSteps.Add(step);
			step.Init();
		}
	}

	protected abstract void Init(Vector2i size);
	protected abstract void BeginRender(CommandList list, Texture rt);
	protected abstract void EndRender(CommandList list, Texture rt);

	public void Render(Texture rt, CameraNode camera)
	{
		UpdateView(List, camera);
		BeginRender(List, rt);

		foreach (var step in renderSteps)
		{
			List.BeginEvent(step.GetType().Name);
			step.RP = (TSelf)this;
			step.Camera = camera;

			step.Run(step.RP.List);
			List.EndEvent();
		}

		EndRender(List, rt);
	}

	public virtual void Dispose()
	{
		List.Dispose();
		ViewCB.Dispose();
	}

	public Matrix4 ViewMatrix { get; private set; }
	public Matrix4 ProjectionMatrix { get; private set; }

	public void UpdateView(CommandList list, CameraNode camera)
	{
		// Calculate view/projection matrices.
		ViewMatrix = camera.WorldTransform.Inverse();
		ProjectionMatrix = Matrix4.CreatePerspectiveReversed(camera.FOV, Size.X / (float)Size.Y, 0.01f);

		// Apply Z-up projection.
		ProjectionMatrix = Matrix4.CreateRotation(new(-90, 180, 0)) * ProjectionMatrix;

		// Upload to constant buffer.
		list.UploadBuffer(ViewCB, new ViewConstants()
		{
			WorldToView = ViewMatrix,
			ViewToWorld = ViewMatrix.Inverse(),
			ViewToClip = ProjectionMatrix,
			ClipToView = ProjectionMatrix.Inverse(),
			EyePosition = camera.WorldTransform.ExtractTranslation(),
		});
	}
}