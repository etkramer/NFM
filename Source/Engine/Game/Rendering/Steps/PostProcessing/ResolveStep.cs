using System;
using Engine.GPU;
using Engine.Resources;
using Engine.World;

namespace Engine.Rendering
{
	public class ResolveStep : RenderStep
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
			List.SetProgramUAV(0, 0, Viewport.ColorTarget);
			List.DispatchThreads(Viewport.ColorTarget.Width, 32, Viewport.ColorTarget.Height, 32);

			// Copy output to backbuffer.
			List.ResolveTexture(Viewport.ColorTarget, Viewport.Host.Swapchain.RT);

			// Clear viewport targets.
			List.ClearRenderTarget(Viewport.ColorTarget);
			List.ClearDepth(Viewport.DepthBuffer);
		}
	}
}