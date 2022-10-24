using System;
using Engine.GPU;
using Engine.Resources;
using Engine.World;

namespace Engine.Rendering
{
	public class ResolveStep : CameraStep
	{
		private PipelineState gammaCorrectPSO;

		public override void Init()
		{
			gammaCorrectPSO = new PipelineState()
				.UseIncludes(typeof(Game).Assembly)
				.SetComputeShader(Embed.GetString("Content/Shaders/PostProcess/GammaCorrectCS.hlsl", typeof(Game).Assembly), "GammaCorrectCS")
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