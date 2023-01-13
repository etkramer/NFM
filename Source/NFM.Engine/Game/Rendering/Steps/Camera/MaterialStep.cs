using System;
using NFM.GPU;
using NFM.Resources;
using NFM.World;
using Vortice.DXGI;

namespace NFM.Rendering
{
	public class MaterialStep : CameraStep
	{
		private GraphicsBuffer commandBuffer;
		private PipelineState cullPSO;

		public override void Init()
		{
			// Compile indirect compute program.
			cullPSO = new PipelineState()
				.UseIncludes(typeof(Engine).Assembly)
				.SetComputeShader(Embed.GetString("Shaders/Geometry/Shared/CullCS.hlsl", typeof(Engine).Assembly), "CullCS")
				.AsRootConstant(0, 1)
				.Compile().Result;

			const int commandStride = 16;
			commandBuffer = new GraphicsBuffer(commandStride * Scene.MaxInstances, commandStride, hasCounter: true);
		}

		public override void Run()
		{
			// Loop through materials to shade.
			foreach (var permutation in ShaderPermutation.All)
			{
				int shaderID = permutation.ShaderID;

				// Build indirect draw commands for this shader ID.
				BuildDraws(shaderID);

				List.BeginEvent($"Draw shader {shaderID}");

				// Switch to material program.
				List.SetPipelineState(permutation.MaterialPSO);

				// Set and reset render targets.
				List.SetRenderTarget(RT.ColorTarget, RT.DepthBuffer);

				// Bind program SRVs.
				List.SetPipelineSRV(0, 1, Mesh.VertBuffer);
				List.SetPipelineSRV(1, 1, Mesh.PrimBuffer);
				List.SetPipelineSRV(2, 1, Mesh.MeshletBuffer);
				List.SetPipelineSRV(3, 1, Mesh.MeshBuffer);
				List.SetPipelineSRV(4, 1, Camera.Scene.TransformBuffer);
				List.SetPipelineSRV(5, 1, Camera.Scene.InstanceBuffer);
				List.SetPipelineSRV(0, 0, MaterialInstance.MaterialBuffer);

				// Bind program CBVs.
				List.SetPipelineCBV(0, 1, RT.ViewCB);

				List.ExecuteIndirect(permutation.MaterialSignature, commandBuffer, (int)Camera.Scene.InstanceBuffer.NumAllocations);
				List.EndEvent();
			}
		}

		private void BuildDraws(int shaderID)
		{
			// Reset command count.
			List.ResetCounter(commandBuffer);

			// Switch to culling program (compute).
			List.SetPipelineState(cullPSO);

			// Set SRV inputs.
			List.SetPipelineSRV(0, 0, MaterialInstance.MaterialBuffer);
			List.SetPipelineSRV(3, 1, Mesh.MeshBuffer);
			List.SetPipelineSRV(5, 1, Camera.Scene.InstanceBuffer);

			// Set UAV outputs.
			List.SetPipelineUAV(0, 0, commandBuffer);

			// Build for chosen shader.
			List.SetPipelineConstants(0, 0, shaderID);

			// Dispatch compute shader.
			if (Camera.Scene.InstanceBuffer.NumAllocations > 0)
			{
				List.Dispatch((int)Camera.Scene.InstanceBuffer.LastOffset + 1);
			}

			List.BarrierUAV(commandBuffer);
		}
	}
}