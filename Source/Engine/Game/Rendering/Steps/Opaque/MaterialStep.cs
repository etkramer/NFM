using System;
using Engine.Content;
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
				.UseIncludes(typeof(Embed).Assembly)
				.SetComputeShader(Embed.GetString("HLSL/Prepass/CullCS.hlsl"), "CullCS")
				.AsRootConstant(0, 1)
				.Compile().Result;

			const int commandStride = 16;
			commandBuffer = new GraphicsBuffer(commandStride * ModelActor.MaxInstanceCount, commandStride, hasCounter: true);
		}

		public override void Run()
		{
			// Reset render targets.
			List.ClearRenderTarget(Viewport.MatBuffer0);
			List.ClearRenderTarget(Viewport.MatBuffer1);

			// Loop through materials to shade.
			foreach (var shaderPair in ShaderStack.Programs)
			{
				CommandSignature commandSignature = shaderPair.Item2.Item2;
				ShaderProgram program = shaderPair.Item2.Item1;
				int shaderID = shaderPair.Item1.ProgramID;

				// Build indirect draw commands for this shader ID.
				BuildDraws(shaderID);

				List.PushEvent($"Draw shader {shaderID}");

				// Switch to material program.
				List.SetProgram(program);

				// Set and reset render targets.
				List.SetRenderTargets(Viewport.DepthBuffer, Viewport.MatBuffer0, Viewport.MatBuffer1);

				// Bind program SRVs.
				List.SetProgramSRV(0, 1, Mesh.VertBuffer);
				List.SetProgramSRV(1, 1, Mesh.PrimBuffer);
				List.SetProgramSRV(2, 1, Mesh.MeshletBuffer);
				List.SetProgramSRV(3, 1, Mesh.MeshBuffer);
				List.SetProgramSRV(4, 1, Actor.TransformBuffer);
				List.SetProgramSRV(5, 1, ModelActor.InstanceBuffer);
				List.SetProgramSRV(0, 0, MaterialInstance.MaterialBuffer);

				// Bind program CBVs.
				List.SetProgramCBV(0, 1, Viewport.ViewCB);

				List.ExecuteIndirect(commandSignature, commandBuffer, ModelActor.InstanceCount);
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
			List.SetProgramSRV(5, 1, ModelActor.InstanceBuffer);

			// Set UAV outputs.
			List.SetProgramUAV(0, 0, commandBuffer);

			// Build for chosen shader.
			List.SetProgramConstants(0, shaderID);

			// Dispatch compute shader.
			if (ModelActor.InstanceCount > 0)
			{
				List.DispatchGroups(ModelActor.InstanceCount);
			}
		}
	}
}