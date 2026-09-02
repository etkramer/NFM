using NFM.GPU;

namespace NFM.Graphics;

class LightingStep : ViewPass
{
	private static PipelineState? lightingPSO;

	private readonly StandardResources resources;
	private readonly ClusterStep clusterStep;

	public LightingStep(StandardResources resources, ClusterStep clusterStep)
	{
		this.resources = resources;
		this.clusterStep = clusterStep;
	}

	public override void Setup(RenderGraphBuilder builder)
	{
		builder.Read(resources.MatBuffer0, resources.MatBuffer1, resources.MatBuffer2, resources.MatBuffer3, resources.DepthBuffer, resources.ShadowMask);
		builder.Write(resources.SceneColor);
	}

	public override void Init(RenderGraph graph)
	{
		// Compile indirect compute program.
		lightingPSO ??= new PipelineState()
			.SetComputeShader(new ShaderModule(Embed.GetString("Shaders/Standard/LightingCS.hlsl"), ShaderStage.Compute))
			.AsRootConstant(0, 1)
			.Compile().Result;
	}

	public override void Run(in ViewPassContext ctx)
	{
		var list = ctx.List;
		var scene = ctx.RenderScene;
		var sceneColor = ctx.Get(resources.SceneColor);

		list.SetPipelineState(Guard.NotNull(lightingPSO));

		list.SetPipelineUAV(0, 0, sceneColor);
		list.SetPipelineSRV(0, 0, ctx.Get(resources.MatBuffer0));
		list.SetPipelineSRV(1, 0, ctx.Get(resources.MatBuffer1));
		list.SetPipelineSRV(2, 0, ctx.Get(resources.MatBuffer2));
		list.SetPipelineSRV(3, 0, ctx.Get(resources.MatBuffer3));
		list.SetPipelineSRV(4, 0, ctx.Get(resources.DepthBuffer));
		list.SetPipelineSRV(5, 0, ctx.Get(resources.ShadowMask));
		list.SetPipelineSRV(6, 1, scene.LightBuffer);
		list.SetPipelineSRV(9, 1, clusterStep.Counts);
		list.SetPipelineSRV(10, 1, clusterStep.Offsets);
		list.SetPipelineSRV(11, 1, clusterStep.Lights);
		list.SetPipelineCBV(0, 1, ctx.ViewCB);

		list.SetPipelineConstants(0, 0, (int)ctx.Camera.DisplayMode);

		list.DispatchThreads(sceneColor.Width, 8, sceneColor.Height, 8);
	}
}
