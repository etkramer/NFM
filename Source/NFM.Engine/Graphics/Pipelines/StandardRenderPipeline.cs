using NFM.GPU;
using Vortice.DXGI;

namespace NFM.Graphics;

/// <summary>
/// Handles to every texture in the standard pipeline, handed to each of its passes.
/// </summary>
class StandardResources
{
	public required TextureHandle SceneColor { get; init; } // Linear HDR radiance, pre-tonemap
	public required TextureHandle ColorTarget { get; init; } // Display-encoded output

	public required TextureHandle VisBuffer { get; init; }
	public required TextureHandle DepthBuffer { get; init; }

	public required TextureHandle MatBuffer0 { get; init; } // RGB: Albedo
	public required TextureHandle MatBuffer1 { get; init; } // RGB: Normal
	public required TextureHandle MatBuffer2 { get; init; } // R: Metallic, G: Specular, B: Roughness
	public required TextureHandle MatBuffer3 { get; init; } // RGB: Emissive radiance
}

class StandardRenderPipeline : RenderPipeline
{
	private StandardResources resources = null!;

	protected override void Setup()
	{
		resources = new StandardResources()
		{
			SceneColor = Graph.CreateTexture("Scene Color", new(Size, Format.R16G16B16A16_Float)),
			ColorTarget = Graph.CreateTexture("Color Target", new(Size, Format.R8G8B8A8_UNorm)),

			VisBuffer = Graph.CreateTexture("Vis Buffer", new(Size, Format.R32G32_UInt)),
			DepthBuffer = Graph.CreateTexture("Depth Buffer", new(Size, Format.R32_Typeless, Format.D32_Float, Format.R32_Float)),

			MatBuffer0 = Graph.CreateTexture("Material Buffer 0", new(Size, Format.R8G8B8A8_UNorm)),
			MatBuffer1 = Graph.CreateTexture("Material Buffer 1", new(Size, Format.R16G16B16A16_Float)),
			MatBuffer2 = Graph.CreateTexture("Material Buffer 2", new(Size, Format.R8G8B8A8_UNorm)),
			MatBuffer3 = Graph.CreateTexture("Material Buffer 3", new(Size, Format.R11G11B10_Float)),
		};

		Graph.AddPass(new PrepassStep(resources));
		Graph.AddPass(new PickingStep(resources));
		Graph.AddPass(new MaterialStep(resources));
		Graph.AddPass(new LightingStep(resources));
		Graph.AddPass(new TonemapStep(resources));
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
