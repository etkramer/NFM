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

			// Bind common inputs.
			Graphics.SetProgramSRV(252, ModelActor.InstanceBuffer);
			Graphics.SetProgramSRV(253, Submesh.MeshBuffer);
			Graphics.SetProgramSRV(254, Submesh.MeshletBuffer);
			Graphics.SetProgramSRV(255, Submesh.PrimBuffer);
			Graphics.SetProgramSRV(256, Submesh.VertBuffer);

			// Bind program inputs/outputs
			Graphics.SetProgramSRV(0, Viewport.VisBuffer);
			Graphics.SetProgramSRV(1, Viewport.DepthBuffer);
			Graphics.SetProgramUAV(0, Viewport.ColorTarget);

			// Dispatch the material program.
			Graphics.DispatchThreads(Viewport.ColorTarget.Width, 32, Viewport.ColorTarget.Height, 32);

			// Wait until all writes are complete before proceeding.
			Graphics.BarrierUAV(Viewport.ColorTarget);
		}
	}
}