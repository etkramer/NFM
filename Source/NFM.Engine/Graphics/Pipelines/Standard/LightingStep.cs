using NFM.GPU;

namespace NFM.Graphics;

class LightingStep : ViewPass
{
	private static PipelineState? lightingPSO;

	private readonly StandardResources resources;

	public LightingStep(StandardResources resources)
	{
		this.resources = resources;
	}

	public override void Setup(RenderGraphBuilder builder)
	{
		builder.Read(resources.MatBuffer0, resources.MatBuffer1, resources.MatBuffer2, resources.DepthBuffer);
		builder.Write(resources.ColorTarget);
	}

	public override void Init(RenderGraph graph)
	{
		// Compile indirect compute program.
		lightingPSO ??= new PipelineState()
			.SetComputeShader(new ShaderModule(Embed.GetString("Shaders/Standard/LightingCS.hlsl"), ShaderStage.Compute))
			.Compile().Result;
	}

	public override void Run(in ViewPassContext ctx)
	{
		var list = ctx.List;
		var colorTarget = ctx.Get(resources.ColorTarget);

		list.SetPipelineState(Guard.NotNull(lightingPSO));

		list.SetPipelineUAV(0, 0, colorTarget);
		list.SetPipelineSRV(0, 0, ctx.Get(resources.MatBuffer0));
		list.SetPipelineSRV(1, 0, ctx.Get(resources.MatBuffer1));
		list.SetPipelineSRV(2, 0, ctx.Get(resources.MatBuffer2));
		list.SetPipelineSRV(3, 0, ctx.Get(resources.DepthBuffer));

		list.DispatchThreads(colorTarget.Width, 32, colorTarget.Height, 32);
	}
}
