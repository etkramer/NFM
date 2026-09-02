using NFM.GPU;

namespace NFM.Graphics;

/// <summary>
/// Traces one shadow ray per light per pixel, into a per-pixel visibility bitmask.
/// </summary>
class ShadowStep : ViewPass
{
	private static PipelineState? shadowPSO;

	private readonly StandardResources resources;

	public ShadowStep(StandardResources resources)
	{
		this.resources = resources;
	}

	public override void Setup(RenderGraphBuilder builder)
	{
		builder.Read(resources.MatBuffer1, resources.DepthBuffer);
		builder.Read(resources.Clusters.Counts, resources.Clusters.Offsets, resources.Clusters.Lights);
		builder.Write(resources.ShadowMask);
	}

	public override void Init(RenderGraph graph)
	{
		shadowPSO ??= new PipelineState()
			.SetComputeShader(new ShaderModule(Embed.GetString("Shaders/Standard/ShadowCS.hlsl"), ShaderStage.Compute))
			.Compile().Result;
	}

	public override void Run(in ViewPassContext ctx)
	{
		var list = ctx.List;
		var scene = ctx.RenderScene;
		var shadowMask = ctx.Get(resources.ShadowMask);

		list.SetPipelineState(Guard.NotNull(shadowPSO));

		list.SetPipelineUAV(0, 0, shadowMask);
		list.SetPipelineSRV(0, 0, ctx.Get(resources.MatBuffer1));
		list.SetPipelineSRV(1, 0, ctx.Get(resources.DepthBuffer));
		list.SetPipelineSRV(6, 1, scene.LightBuffer);
		list.SetPipelineSRV(8, 1, scene.TLAS.Structure);
		list.SetPipelineSRV(9, 1, ctx.Get(resources.Clusters.Counts));
		list.SetPipelineSRV(10, 1, ctx.Get(resources.Clusters.Offsets));
		list.SetPipelineSRV(11, 1, ctx.Get(resources.Clusters.Lights));
		list.SetPipelineCBV(0, 1, ctx.ViewCB);

		list.DispatchThreads(shadowMask.Width, 8, shadowMask.Height, 8);
	}
}
