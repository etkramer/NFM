using System;
using NFM.GPU;
using NFM.Resources;
using NFM.World;

namespace NFM.Graphics;

public class PreviewStep : CameraStep<VisibilityRenderPipeline>
{
	private PipelineState previewPSO;

	public override void Init()
	{
		// Compile depth prepass program.
		previewPSO = new PipelineState()
			.UseIncludes(typeof(Engine).Assembly)
			.SetComputeShader(Embed.GetString("Shaders/Visibility/PreviewCS.hlsl", typeof(Engine).Assembly), "PreviewCS")
			.Compile().Result;
	}

	public override void Run(CommandList list)
	{
		list.SetPipelineState(previewPSO);
		list.SetPipelineUAV(0, 0, RP.ColorTarget);
		list.SetPipelineSRV(0, 0, RP.VisBuffer);
		list.SetPipelineSRV(1, 0, RP.DepthBuffer);
		list.DispatchThreads(RP.ColorTarget.Width, 32, RP.ColorTarget.Height, 32);
	}
}