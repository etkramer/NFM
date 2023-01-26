using System;
using NFM.GPU;
using NFM.Resources;
using NFM.World;

namespace NFM.Graphics;

public class PrepassStep : CameraStep<StandardRenderPipeline>
{
	public GraphicsBuffer CommandBuffer;
	public CommandSignature CommandSignature;

	private PipelineState cullPSO;
	private PipelineState visPSO;

	public override void Init()
	{
		// Compile indirect compute program.
		cullPSO = new PipelineState()
			.UseIncludes(typeof(Engine).Assembly)
			.SetComputeShader(Embed.GetString("Shaders/Standard/CullCS.hlsl", typeof(Engine).Assembly), "CullCS")
			.Compile().Result;

		// Compile depth prepass program.
		visPSO = new PipelineState()
			.UseIncludes(typeof(Engine).Assembly)
			.SetMeshShader(Embed.GetString("Shaders/Standard/Prepass/BaseMS.hlsl", typeof(Engine).Assembly), "BaseMS")
			.SetPixelShader(Embed.GetString("Shaders/Standard/Prepass/PrepassPS.hlsl", typeof(Engine).Assembly), "PrepassPS")
			.AsRootConstant(0, 1)
			.SetDepthMode(DepthMode.GreaterEqual, true, true)
			.SetCullMode(CullMode.CCW)
			.SetRTFormat(0, Vortice.DXGI.Format.R32G32_UInt)
			.Compile().Result;

		// Indirect command signature for depth pass.
		CommandSignature = new CommandSignature()
			.AddConstantArg(0, visPSO)
			.AddDispatchMeshArg()
			.Compile();

		CommandBuffer = new GraphicsBuffer(CommandSignature.Stride * Scene.MaxInstances, CommandSignature.Stride, hasCounter: true);
	}

	public override void Run(CommandList list)
	{
		// Generate indirect draw commands and sort front-back.
		BuildIndirectCommands(list);

		// Build depth buffer for opaque geometry.
		DrawVisibility(list);
	}

	private void BuildIndirectCommands(CommandList list)
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

		// Dispatch compute shader.
		if (Camera.Scene.InstanceBuffer.NumAllocations > 0)
		{
			list.Dispatch((int)Camera.Scene.InstanceBuffer.LastOffset + 1);
		}

		list.BarrierUAV(CommandBuffer);
	}

	private void DrawVisibility(CommandList list)
	{
		// Switch to material program.
		list.SetPipelineState(visPSO);

		// Set render targets.
		list.SetRenderTarget(RP.VisBuffer, RP.DepthBuffer);

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
		list.ExecuteIndirect(CommandSignature, CommandBuffer, (int)Camera.Scene.InstanceBuffer.NumAllocations);
	}
}