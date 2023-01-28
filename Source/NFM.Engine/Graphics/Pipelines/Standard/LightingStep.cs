using System;
using NFM.GPU;
using NFM.Resources;

namespace NFM.Graphics;

public class LightingStep : CameraStep<StandardRenderPipeline>
{
	private PipelineState lightingPSO;

	public override void Init()
	{
		// Compile indirect compute program.
		lightingPSO = new PipelineState()
			.SetComputeShader(new ShaderModule(Embed.GetString("Shaders/Standard/LightingCS.hlsl"), ShaderStage.Compute))
			.Compile().Result;
	}

	public override void Run(CommandList list)
	{
		list.SetPipelineState(lightingPSO);
	
		list.SetPipelineUAV(0, 0, RP.ColorTarget);
		list.SetPipelineSRV(0, 0, RP.MatBuffer0);
		list.SetPipelineSRV(1, 0, RP.MatBuffer1);
		list.SetPipelineSRV(2, 0, RP.MatBuffer2);
		list.SetPipelineSRV(3, 0, RP.DepthBuffer);

		list.DispatchThreads(RP.ColorTarget.Width, 32, RP.ColorTarget.Height, 32);
	}
}