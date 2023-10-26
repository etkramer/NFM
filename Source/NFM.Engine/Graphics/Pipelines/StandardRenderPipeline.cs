using NFM.GPU;
using Vortice.DXGI;

namespace NFM.Graphics;

class StandardRenderPipeline : RenderPipeline<StandardRenderPipeline>
{
	public Texture ColorTarget;

	public Texture VisBuffer;
	public Texture DepthBuffer;

	public Texture MatBuffer0; // RGB: Albedo
	public Texture MatBuffer1; // RGB: Normal
	public Texture MatBuffer2; // R: Metallic, G: Specular, B: Roughness

	protected override void Init(Vector2i size)
	{
		AddStep<PrepassStep>();
		AddStep<MaterialStep>();
		AddStep<LightingStep>();
	
		ColorTarget = new Texture(size.X, size.Y, 1, Format.R8G8B8A8_UNorm);

		VisBuffer = new Texture(size.X, size.Y, 1, Format.R32G32_UInt);
		DepthBuffer = new Texture(size.X, size.Y, 1, Format.R32_Typeless, dsFormat: Format.D32_Float, srFormat: Format.R32_Float);

		MatBuffer0 = new Texture(size.X, size.Y, 1, Format.R8G8B8A8_UNorm);
		MatBuffer1 = new Texture(size.X, size.Y, 1, Format.R16G16B16A16_Float);
		MatBuffer2 = new Texture(size.X, size.Y, 1, Format.R8G8B8A8_UNorm);
	}

	protected override void BeginRender(CommandList list, Texture rt)
	{
		list.ClearDepth(DepthBuffer);
	}

	protected override void EndRender(CommandList list, Texture	rt)
	{
		list.ResolveTexture(ColorTarget, rt);
	}

	public override void Dispose()
	{
		ColorTarget.Dispose();
		VisBuffer.Dispose();
		DepthBuffer.Dispose();

		MatBuffer0.Dispose();
		MatBuffer1.Dispose();
		MatBuffer2.Dispose();

		base.Dispose();
	}
}