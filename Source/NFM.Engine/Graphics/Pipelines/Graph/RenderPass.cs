using NFM.GPU;
using NFM.World;

namespace NFM.Graphics;

/// <summary>
/// A pass over a whole scene, run once per frame before any views are rendered.
/// </summary>
abstract class ScenePass : IDisposable
{
	public virtual void Init() {}
	public abstract void Run(in ScenePassContext ctx);
	public virtual void Dispose() {}
}

/// <summary>
/// A pass over a single view. Declares the graph textures it touches in <see cref="Setup"/>.
/// </summary>
abstract class ViewPass : IDisposable
{
	public virtual void Setup(RenderGraphBuilder builder) {}
	public virtual void Init(RenderGraph graph) {}
	public abstract void Run(in ViewPassContext ctx);
	public virtual void Dispose() {}
}

readonly struct ScenePassContext
{
	public required CommandList List { get; init; }
	public required Scene Scene { get; init; }

	public RenderScene RenderScene => Scene.RenderData;
}

readonly struct ViewPassContext
{
	public required CommandList List { get; init; }
	public required CameraNode Camera { get; init; }
	public required RenderGraph Graph { get; init; }
	public required TypedBuffer<ViewConstants> ViewCB { get; init; }

	public RenderScene RenderScene => Camera.Scene.RenderData;

	public Texture Get(TextureHandle handle) => Graph.Get(handle);
	public RawBuffer Get(BufferHandle handle) => Graph.Get(handle);
}
