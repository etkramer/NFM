using System;
using NFM.GPU;
using NFM.Resources;
using NFM.World;

namespace NFM.Rendering
{
	public class PrepassStep : CameraStep
	{
		public GraphicsBuffer CommandBuffer;
		public CommandSignature DepthCommandSignature;

		private PipelineState cullPSO;
		private PipelineState depthPSO;

		public override void Init()
		{
			// Compile indirect compute program.
			cullPSO = new PipelineState()
				.UseIncludes(typeof(Engine).Assembly)
				.SetComputeShader(Embed.GetString("Content/Shaders/Geometry/Shared/CullCS.hlsl", typeof(Engine).Assembly), "CullCS")
				.AsRootConstant(0, 1)
				.Compile().Result;

			// Compile depth prepass program.
			depthPSO = new PipelineState()
				.UseIncludes(typeof(Engine).Assembly)
				.SetMeshShader(Embed.GetString("Content/Shaders/Geometry/Shared/BaseMS.hlsl", typeof(Engine).Assembly), "BaseMS")
				.SetPixelShader(Embed.GetString("Content/Shaders/Geometry/Prepass/DepthPS.hlsl", typeof(Engine).Assembly), "DepthPS")
				.SetDepthMode(DepthMode.GreaterEqual, true, true)
				.SetCullMode(CullMode.CCW)
				.AsRootConstant(0, 1)
				.Compile().Result;

			// Indirect command signature for depth pass.
			DepthCommandSignature = new CommandSignature()
				.AddConstantArg(0, depthPSO)
				.AddDispatchMeshArg()
				.Compile();

			CommandBuffer = new GraphicsBuffer(DepthCommandSignature.Stride * Scene.MaxInstances, DepthCommandSignature.Stride, hasCounter: true);
		}

		public override void Run()
		{
			// Generate indirect draw commands and sort front-back.
			Cull();

			// Build depth buffer for opaque geometry.
			DrawDepth();
		}

		private void Cull()
		{
			// Reset command count.
			List.ResetCounter(CommandBuffer);

			// Switch to culling program (compute).
			List.SetPipelineState(cullPSO);

			// Set SRV inputs.
			List.SetPipelineSRV(3, 1, Mesh.MeshBuffer);
			List.SetPipelineSRV(5, 1, Camera.Scene.InstanceBuffer);

			// Set UAV outputs.
			List.SetPipelineUAV(0, 0, CommandBuffer);

			// Build for all shaders
			List.SetPipelineConstants(0, 0, -1);

			// Dispatch compute shader.
			if (Camera.Scene.InstanceBuffer.NumAllocations > 0)
			{
				List.Dispatch((int)Camera.Scene.InstanceBuffer.LastOffset + 1);
			}

			List.BarrierUAV(CommandBuffer);
		}

		private void DrawDepth()
		{
			// Switch to material program.
			List.SetPipelineState(depthPSO);

			// Set render targets.
			List.SetRenderTarget(null, RT.DepthBuffer);

			// Bind program SRVs.
			List.SetPipelineSRV(0, 1, Mesh.VertBuffer);
			List.SetPipelineSRV(1, 1, Mesh.PrimBuffer);
			List.SetPipelineSRV(2, 1, Mesh.MeshletBuffer);
			List.SetPipelineSRV(3, 1, Mesh.MeshBuffer);
			List.SetPipelineSRV(4, 1, Camera.Scene.TransformBuffer);
			List.SetPipelineSRV(5, 1, Camera.Scene.InstanceBuffer);

			// Bind program CBVs.
			List.SetPipelineCBV(0, 1, RT.ViewCB);

			// Dispatch draw commands.
			List.ExecuteIndirect(DepthCommandSignature, CommandBuffer, (int)Camera.Scene.InstanceBuffer.NumAllocations);
		}
	}
}