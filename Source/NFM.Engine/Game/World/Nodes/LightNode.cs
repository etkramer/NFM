using NFM.GPU;
using NFM.Graphics;

namespace NFM.World;

/// <summary>
/// Base for anything that emits light. Owns a slot in the scene's light buffer, which is refilled
/// from <see cref="Pack"/> whenever the light is marked dirty.
/// </summary>
[Icon("light_mode")]
public abstract class LightNode : Node
{
	/// <summary>
	/// The light's tint, as a linear RGB multiplier
	/// </summary>
	[Inspect]
	public Vector3 Color { get; set; } = Vector3.One;

	// Display steps a light may still be contributing where it's cut off, and the illuminance one
	// step costs a mid-grey surface at unit exposure.
	private const float VisibleSteps = 8;
	private const float StepIlluminance = MathF.PI / (0.5f * 255 * 12.92f);

	/// <summary>
	/// Reciprocal of the illuminance a light stops being worth evaluating at, for a given exposure.
	/// Ranges are derived from this in the shader, so brightening a view tightens them on its own.
	/// </summary>
	public static float InvCutoffFor(float exposure) => MathF.Max(exposure, 1e-6f) / (VisibleSteps * StepIlluminance);

	internal BufferAllocation<GPULight> LightHandle;

	private RenderScene RenderScene => Scene.RenderData;

	protected LightNode(Scene? scene) : base(scene)
	{
		Name = "Light";

		LightHandle = RenderScene.LightBuffer.Allocate(1, true);

		this.SubscribeFast(nameof(Color), nameof(WorldTransform), MarkDirty);
		MarkDirty();
	}

	/// <summary>
	/// Queues this light for reupload. Subclasses should call it when any of their own properties change.
	/// </summary>
	protected void MarkDirty() => RenderScene.MarkLightDirty(this);

	/// <summary>
	/// Builds the GPU-side representation of this light.
	/// </summary>
	protected abstract GPULight Pack();

	internal void UploadLight(CommandList list)
	{
		list.UploadBuffer(LightHandle, Pack());
	}

	public override void Dispose()
	{
		RenderScene.Forget(this);

		// Zero out light data
		Renderer.DefaultCommandList.UploadBuffer(LightHandle, default(GPULight));
		LightHandle.Dispose();

		base.Dispose();
	}
}
