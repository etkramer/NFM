using System;
using NFM.GPU;
using NFM.Resources;
using NFM.World;

namespace NFM.Graphics;

public class TonemapStep : CameraStep<StandardRenderPipeline>
{
	private PipelineState gammaCorrectPSO;

	public override void Init()
	{
		gammaCorrectPSO = new PipelineState()
			.UseIncludes(typeof(Engine).Assembly)
			.SetComputeShader(Embed.GetString("Shaders/PostProcess/GammaCorrectCS.hlsl", typeof(Engine).Assembly), "GammaCorrectCS")
			.Compile().Result;
	}

	public override void Run()
	{
		// Gamma correct output.
		List.SetPipelineState(gammaCorrectPSO);
		List.SetPipelineUAV(0, 0, RP.ColorTarget);
		List.DispatchThreads(RP.ColorTarget.Width, 32, RP.ColorTarget.Height, 32);
	}
}