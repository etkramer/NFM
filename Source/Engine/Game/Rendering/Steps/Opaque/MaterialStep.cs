using System;
using Engine.Content;
using Engine.GPU;
using Engine.Resources;
using Engine.World;

namespace Engine.Rendering
{
	public class MaterialStep : RenderStep
	{
		private ShaderProgram materialProgram;

		public override void Init()
		{
			materialProgram = new ShaderProgram()
				.UseIncludes(typeof(Embed).Assembly)
				.SetComputeShader(Embed.GetString("Shaders/MaterialShader.hlsl"))
				.Compile().Result;
		}

		public override void Run()
		{
			// Shade objects with material shaders.
			DrawMaterials();
		}

		private void DrawMaterials()
		{
			// Switch to material program (compute).
			Graphics.SetProgram(materialProgram);

			// Bind program inputs/outputs
			Graphics.SetProgramSRV(1, Viewport.DepthBuffer);
			//Graphics.SetProgramUAV(0, Viewport.ColorTarget);

			// Dispatch draw commands.
			Graphics.Dispatch(Viewport.ColorTarget.Width / 32, Viewport.ColorTarget.Height / 32);
		}
	}
}