using NFM.GPU;

namespace NFM.Graphics;

/// <summary>
/// Traces one shadow ray per light per pixel, packing the results into a visibility bitmask for the
/// lighting pass to resolve.
/// </summary>
class ShadowStep : ViewPass
{
	/// <summary>
	/// Lights past this many go unshadowed, being beyond what one mask can hold.
	/// </summary>
	public const int MaxShadowedLights = 32;

	private static PipelineState? shadowPSO;

	private readonly StandardResources resources;

	public ShadowStep(StandardResources resources)
	{
		this.resources = resources;
	}

	public override void Setup(RenderGraphBuilder builder)
	{
		builder.Read(resources.MatBuffer1, resources.DepthBuffer);
		builder.Write(resources.ShadowMask);
	}

	public override void Init(RenderGraph graph)
	{
		shadowPSO ??= new PipelineState()
			.SetComputeShader(new ShaderModule(Embed.GetString("Shaders/Standard/ShadowCS.hlsl"), ShaderStage.Compute))
			.AsRootConstant(0, 1)
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
		list.SetPipelineCBV(0, 1, ctx.ViewCB);

		list.SetPipelineConstants(0, 0, scene.LightCount);

		list.DispatchThreads(shadowMask.Width, 8, shadowMask.Height, 8);
	}
}
