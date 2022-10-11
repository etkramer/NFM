using System;
using Engine.GPU;
using Engine.Resources;
using Engine.World;

namespace Engine.Rendering
{
	public class ResolveStep : CameraStep
	{
		private ShaderProgram gammaProgram;

		public override void Init()
		{
			gammaProgram = new ShaderProgram()
				.UseIncludes(typeof(Game).Assembly)
				.SetComputeShader(Embed.GetString("Content/Shaders/PostProcess/GammaCorrectCS.hlsl", typeof(Game).Assembly), "GammaCorrectCS")
				.Compile().Result;
		}

		public override void Run()
		{
			// Gamma correct output.
			List.SetProgram(gammaProgram);
			List.SetProgramUAV(0, 0, RT.ColorTarget);
			List.DispatchThreads(RT.ColorTarget.Width, 32, RT.ColorTarget.Height, 32);
		}
	}
}