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
	public float ClusterScale; // Slice distribution, as log2(depth) * scale + bias

	public Vector3i ClusterDims;
	public float ClusterBias;

	public float InvLightCutoff; // Reciprocal of the illuminance a light stops being worth evaluating at
}

/// <summary>
/// Describes a series of passes for rendering an image.
/// Owns any size-dependent resources - gbuffers, post-processing targets, etc.
/// One instance belongs to exactly one camera, so passes are free to keep per-view history.
/// </summary>
abstract class RenderPipeline : IDisposable
{
	public TypedBuffer<ViewConstants> ViewCB { get; } = new(1, RawBuffer.ConstantAlignment);

	public CommandList List { get; } = new CommandList();
	public Vector2i Size { get; private set; } = default;

	protected RenderGraph Graph { get; } = new();

	public Matrix4 ViewMatrix { get; private set; }
	public Matrix4 ProjectionMatrix { get; private set; }

	/// <summary>
	/// Declares the pipeline's textures and passes. Called once, before the first frame.
	/// </summary>
	protected abstract void Setup();

	protected abstract void BeginRender(CommandList list, Texture rt);
	protected abstract void EndRender(CommandList list, Texture rt);

	/// <summary>
	/// Renders this pipeline's graph as text, for logging or inspection.
	/// </summary>
	public string Describe() => Graph.Describe();

	public void Build(Vector2i size)
	{
		Size = size;

		Setup();
		Graph.Build();
	}

	public void Render(Texture rt, CameraNode camera)
	{
		UpdateView(List, camera);
		BeginRender(List, rt);

		ViewPassContext ctx = new()
		{
			List = List,
			Camera = camera,
			Graph = Graph,
			ViewCB = ViewCB
		};

		foreach (var pass in Graph.Passes)
		{
			List.BeginEvent(pass.GetType().Name);
			pass.Run(ctx);
			List.EndEvent();
		}

		EndRender(List, rt);
	}

	public void UpdateView(CommandList list, CameraNode camera)
	{
		// Calculate view/projection matrices.
		ViewMatrix = camera.WorldTransform.Inverse();
		ProjectionMatrix = Matrix4.CreatePerspectiveReversed(camera.FOV, Size.X / (float)Size.Y, 0.01f);

		// Apply Z-up projection.
		ProjectionMatrix = Matrix4.CreateRotation(new(-90, 180, 0)) * ProjectionMatrix;

		float logRange = MathF.Log2(ClusterStep.FarDepth / ClusterStep.NearDepth);

		// Upload to constant buffer.
		list.UploadBuffer(ViewCB, new ViewConstants()
		{
			WorldToView = ViewMatrix,
			ViewToWorld = ViewMatrix.Inverse(),
			ViewToClip = ProjectionMatrix,
			ClipToView = ProjectionMatrix.Inverse(),
			EyePosition = camera.WorldTransform.ExtractTranslation(),

			ClusterDims = ClusterStep.DimensionsFor(Size),
			ClusterScale = ClusterStep.SliceCount / logRange,
			ClusterBias = -ClusterStep.SliceCount * MathF.Log2(ClusterStep.NearDepth) / logRange,

			InvLightCutoff = LightNode.InvCutoffFor(camera.Exposure),
		});
	}

	public virtual void Dispose()
	{
		List.Dispose();
		ViewCB.Dispose();
		Graph.Dispose();
	}
}
