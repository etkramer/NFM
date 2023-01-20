using System;
using NFM.GPU;
using Vortice.DXGI;

namespace NFM.Graphics;

public class StandardRenderPipeline : RenderPipeline<StandardRenderPipeline>
{
	public Texture ColorTarget;
	public Texture DepthBuffer;

	protected override void Init(Vector2i size)
	{
		AddStep<PrepassStep>();
		AddStep<MaterialStep>();
		AddStep<TonemapStep>();

		// Create RTs and RT-sized buffers.
		ColorTarget = new Texture(size.X, size.Y, 1, Format.R8G8B8A8_UNorm, samples: 1);
		DepthBuffer = new Texture(size.X, size.Y, 1, Format.R32_Typeless, dsFormat: Format.D32_Float, srFormat: Format.R32_Float, samples: 1);
	}

	protected override void BeginRender(CommandList list, Texture rt)
	{
		list.ClearRenderTarget(ColorTarget);
		list.ClearDepth(DepthBuffer);
	}

	protected override void EndRender(CommandList list, Texture	rt)
	{
		list.ResolveTexture(ColorTarget, rt);
	}

	public override void Dispose()
	{
		ColorTarget.Dispose();
		DepthBuffer.Dispose();

		base.Dispose();
	}
}