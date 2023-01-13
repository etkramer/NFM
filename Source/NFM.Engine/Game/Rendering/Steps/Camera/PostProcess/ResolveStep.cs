using System;
using NFM.GPU;
using NFM.Resources;
using NFM.World;

namespace NFM.Rendering
{
	public class TonemapStep : CameraStep
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
			List.SetPipelineUAV(0, 0, RT.ColorTarget);
			List.DispatchThreads(RT.ColorTarget.Width, 32, RT.ColorTarget.Height, 32);
		}
	}
}