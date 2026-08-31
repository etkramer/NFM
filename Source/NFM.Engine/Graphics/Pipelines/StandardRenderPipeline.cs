using NFM.GPU;
using Vortice.DXGI;

namespace NFM.Graphics;

/// <summary>
/// Handles to every texture in the standard pipeline, handed to each of its passes.
/// </summary>
class StandardResources
{
	public required TextureHandle ColorTarget { get; init; }

	public required TextureHandle VisBuffer { get; init; }
	public required TextureHandle DepthBuffer { get; init; }

	public required TextureHandle MatBuffer0 { get; init; } // RGB: Albedo
	public required TextureHandle MatBuffer1 { get; init; } // RGB: Normal
	public required TextureHandle MatBuffer2 { get; init; } // R: Metallic, G: Specular, B: Roughness
}

class StandardRenderPipeline : RenderPipeline
{
	private StandardResources resources = null!;

	protected override void Setup()
	{
		resources = new StandardResources()
		{
			ColorTarget = Graph.CreateTexture("Color Target", new(Size, Format.R8G8B8A8_UNorm)),

			VisBuffer = Graph.CreateTexture("Vis Buffer", new(Size, Format.R32G32_UInt)),
			DepthBuffer = Graph.CreateTexture("Depth Buffer", new(Size, Format.R32_Typeless, Format.D32_Float, Format.R32_Float)),

			MatBuffer0 = Graph.CreateTexture("Material Buffer 0", new(Size, Format.R8G8B8A8_UNorm)),
			MatBuffer1 = Graph.CreateTexture("Material Buffer 1", new(Size, Format.R16G16B16A16_Float)),
			MatBuffer2 = Graph.CreateTexture("Material Buffer 2", new(Size, Format.R8G8B8A8_UNorm)),
		};

		Graph.AddPass(new PrepassStep(resources));
		Graph.AddPass(new PickingStep(resources));
		Graph.AddPass(new MaterialStep(resources));
		Graph.AddPass(new LightingStep(resources));
	}

	protected override void BeginRender(CommandList list, Texture rt)
	{
		list.ClearDepth(Graph.Get(resources.DepthBuffer));
	}

	protected override void EndRender(CommandList list, Texture rt)
	{
		list.ResolveTexture(Graph.Get(resources.ColorTarget), rt);
	}
}
