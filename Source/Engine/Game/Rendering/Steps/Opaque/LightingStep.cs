using System;
using Engine.Content;
using Engine.GPU;
using Engine.Resources;
using Engine.World;
using Vortice.DXGI;

namespace Engine.Rendering
{
	public class LightingStep : RenderStep
	{
		private ShaderProgram lightingProgram;

		public override void Init()
		{
			lightingProgram = new ShaderProgram()
				.UseIncludes(typeof(Embed).Assembly)
				.SetComputeShader(Embed.GetString("HLSL/Lighting/LightingCS.hlsl"), "LightingCS")
				.Compile().Result;
		}

		public override void Run()
		{
			// Switch to material program.
			List.SetProgram(lightingProgram);

			// Set program inputs and outputs.
			List.SetProgramSRV(0, 0, Viewport.DepthBuffer);
			List.SetProgramSRV(1, 0, Viewport.MatBuffer0);
			List.SetProgramSRV(2, 0, Viewport.MatBuffer1);
			List.SetProgramUAV(0, 0, Viewport.ColorTarget);
			List.SetProgramCBV(256, 0, Viewport.ViewCB);

			// Dispatch lighting shader.
			List.DispatchThreads(Viewport.ColorTarget.Width, 32, Viewport.ColorTarget.Height, 32);
		}
	}
}