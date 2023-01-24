using System;
using NFM.GPU;
using NFM.Resources;
using NFM.World;

namespace NFM.Graphics;

public class PrepassStep : CameraStep<StandardRenderPipeline>
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
			.SetComputeShader(Embed.GetString("Shaders/Standard/Prepass/CullCS.hlsl", typeof(Engine).Assembly), "CullCS")
			.AsRootConstant(0, 1)
			.Compile().Result;

		// Compile depth prepass program.
		depthPSO = new PipelineState()
			.UseIncludes(typeof(Engine).Assembly)
			.SetMeshShader(Embed.GetString("Shaders/Standard/Geometry/BaseMS.hlsl", typeof(Engine).Assembly), "BaseMS")
			.SetPixelShader(Embed.GetString("Shaders/Standard/Prepass/DepthPS.hlsl", typeof(Engine).Assembly), "DepthPS")
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

	public override void Run(CommandList list)
	{
		// Generate indirect draw commands and sort front-back.
		Cull(list);

		// Build depth buffer for opaque geometry.
		DrawDepth(list);
	}

	private void Cull(CommandList list)
	{
		// Reset command count.
		list.ResetCounter(CommandBuffer);

		// Switch to culling program (compute).
		list.SetPipelineState(cullPSO);

		// Set SRV inputs.
		list.SetPipelineSRV(3, 1, Mesh.MeshBuffer);
		list.SetPipelineSRV(5, 1, Camera.Scene.InstanceBuffer);

		// Set UAV outputs.
		list.SetPipelineUAV(0, 0, CommandBuffer);

		// Build for all shaders
		list.SetPipelineConstants(0, 0, -1);

		// Dispatch compute shader.
		if (Camera.Scene.InstanceBuffer.NumAllocations > 0)
		{
			list.Dispatch((int)Camera.Scene.InstanceBuffer.LastOffset + 1);
		}

		list.BarrierUAV(CommandBuffer);
	}

	private void DrawDepth(CommandList list)
	{
		// Switch to material program.
		list.SetPipelineState(depthPSO);

		// Set render targets.
		list.SetRenderTarget(null, RP.DepthBuffer);

		// Bind program SRVs.
		list.SetPipelineSRV(0, 1, Mesh.VertBuffer);
		list.SetPipelineSRV(1, 1, Mesh.PrimBuffer);
		list.SetPipelineSRV(2, 1, Mesh.MeshletBuffer);
		list.SetPipelineSRV(3, 1, Mesh.MeshBuffer);
		list.SetPipelineSRV(4, 1, Camera.Scene.TransformBuffer);
		list.SetPipelineSRV(5, 1, Camera.Scene.InstanceBuffer);

		// Bind program CBVs.
		list.SetPipelineCBV(0, 1, RP.ViewCB);

		// Dispatch draw commands.
		list.ExecuteIndirect(DepthCommandSignature, CommandBuffer, (int)Camera.Scene.InstanceBuffer.NumAllocations);
	}
}