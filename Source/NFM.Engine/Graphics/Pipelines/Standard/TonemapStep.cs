using NFM.GPU;

namespace NFM.Graphics;

/// <summary>
/// Resolves linear HDR radiance into the display-encoded color target. Anything that needs to work
/// in HDR - bloom, AA - belongs before this pass.
/// </summary>
class TonemapStep : ViewPass
{
	private static PipelineState? tonemapPSO;

	private readonly StandardResources resources;

	public TonemapStep(StandardResources resources)
	{
		this.resources = resources;
	}

	public override void Setup(RenderGraphBuilder builder)
	{
		builder.Read(resources.SceneColor);
		builder.Write(resources.ColorTarget);
	}

	public override void Init(RenderGraph graph)
	{
		tonemapPSO ??= new PipelineState()
			.SetComputeShader(new ShaderModule(Embed.GetString("Shaders/Standard/TonemapCS.hlsl"), ShaderStage.Compute))
			.AsRootConstant(0, 2)
			.Compile().Result;
	}

	public override void Run(in ViewPassContext ctx)
	{
		var list = ctx.List;
		var colorTarget = ctx.Get(resources.ColorTarget);

		list.SetPipelineState(Guard.NotNull(tonemapPSO));

		list.SetPipelineUAV(0, 0, colorTarget);
		list.SetPipelineSRV(0, 0, ctx.Get(resources.SceneColor));

		// Only the lit view holds radiance - the debug views are already display-referred.
		bool applyTonemap = ctx.Camera.DisplayMode == DisplayMode.Lit;

		list.SetPipelineConstants(0, 0,
			applyTonemap ? 1 : 0,
			BitConverter.SingleToInt32Bits(ctx.Camera.Exposure));

		list.DispatchThreads(colorTarget.Width, 8, colorTarget.Height, 8);
	}
}
