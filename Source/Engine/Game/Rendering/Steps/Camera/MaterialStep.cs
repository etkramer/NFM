using System;
using Engine.GPU;
using Engine.Resources;
using Engine.World;
using Vortice.DXGI;

namespace Engine.Rendering
{
	public class MaterialStep : RenderStep
	{
		private GraphicsBuffer commandBuffer;
		private ShaderProgram cullProgram;

		public override void Init()
		{
			// Compile indirect compute program.
			cullProgram = new ShaderProgram()
				.UseIncludes(typeof(Game).Assembly)
				.SetComputeShader(Embed.GetString("Content/Shaders/Prepass/CullCS.hlsl", typeof(Game).Assembly), "CullCS")
				.AsRootConstant(0, 1)
				.Compile().Result;

			const int commandStride = 16;
			commandBuffer = new GraphicsBuffer(commandStride * Scene.MaxInstanceCount, commandStride, hasCounter: true);
		}

		public override void Run()
		{
			// Loop through materials to shade.
			foreach (var shader in ShaderStack.Stacks)
			{
				int shaderID = shader.ProgramID;

				// Build indirect draw commands for this shader ID.
				BuildDraws(shaderID);

				List.PushEvent($"Draw shader {shaderID}");

				// Switch to material program.
				List.SetProgram(shader.Program);

				// Set and reset render targets.
				List.SetRenderTarget(Viewport.ColorTarget, Viewport.DepthBuffer);

				// Bind program SRVs.
				List.SetProgramSRV(0, 1, Mesh.VertBuffer);
				List.SetProgramSRV(1, 1, Mesh.PrimBuffer);
				List.SetProgramSRV(2, 1, Mesh.MeshletBuffer);
				List.SetProgramSRV(3, 1, Mesh.MeshBuffer);
				List.SetProgramSRV(4, 1, Scene.TransformBuffer);
				List.SetProgramSRV(5, 1, Scene.InstanceBuffer);
				List.SetProgramSRV(0, 0, MaterialInstance.MaterialBuffer);

				// Bind program CBVs.
				List.SetProgramCBV(0, 1, Viewport.ViewCB);

				List.ExecuteIndirect(shader.Signature, commandBuffer, Scene.InstanceCount);
				List.PopEvent();
			}
		}

		private void BuildDraws(int shaderID)
		{
			// Reset command count.
			List.ResetCounter(commandBuffer);

			// Switch to culling program (compute).
			List.SetProgram(cullProgram);

			// Set SRV inputs.
			List.SetProgramSRV(0, 0, MaterialInstance.MaterialBuffer);
			List.SetProgramSRV(3, 1, Mesh.MeshBuffer);
			List.SetProgramSRV(5, 1, Scene.InstanceBuffer);

			// Set UAV outputs.
			List.SetProgramUAV(0, 0, commandBuffer);

			// Build for chosen shader.
			List.SetProgramConstants(0, 0, shaderID);

			// Dispatch compute shader.
			if (Scene.InstanceCount > 0)
			{
				List.DispatchGroups(Scene.InstanceCount);
			}

			List.BarrierUAV(commandBuffer);
		}
	}
}