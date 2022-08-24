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
			List.SetProgram(materialProgram);

			// Bind common inputs.
			List.SetProgramSRV(252, ModelActor.InstanceBuffer);
			List.SetProgramSRV(253, Mesh.MeshBuffer);
			List.SetProgramSRV(254, Mesh.MeshletBuffer);
			List.SetProgramSRV(255, Mesh.PrimBuffer);
			List.SetProgramSRV(256, Mesh.VertBuffer);

			// Bind program inputs/outputs
			List.SetProgramSRV(0, Viewport.VisBuffer);
			List.SetProgramSRV(1, Viewport.DepthBuffer);
			List.SetProgramUAV(0, Viewport.ColorTarget);

			// Bind view constants.
			List.SetProgramCBV(1, Viewport.ViewConstantsBuffer);

			// Dispatch the material program.
			List.DispatchThreads(Viewport.ColorTarget.Width, 8, Viewport.ColorTarget.Height, 8);

			// Wait until all writes are complete before proceeding.
			List.BarrierUAV(Viewport.ColorTarget);
		}
	}
}