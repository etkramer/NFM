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
