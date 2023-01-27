using System;
using NFM.GPU;
using NFM.Resources;
using NFM.World;

namespace NFM.Graphics;

public class PrepassStep : CameraStep<StandardRenderPipeline>
{
	private PipelineState cullPSO;
	private PipelineState visPSO;

	private GraphicsBuffer commandBuffer;
	private CommandSignature commandSignature;

	public override void Init()
	{
		// Compile indirect compute program.
		cullPSO = new PipelineState()
			.SetComputeShader(new ShaderModule(Embed.GetString("Shaders/Standard/CullCS.hlsl"), ShaderStage.Compute))
			.Compile().Result;

		// Compile depth prepass program.
		visPSO = new PipelineState()
			.SetMeshShader(new ShaderModule(Embed.GetString("Shaders/Standard/Prepass/BaseMS.hlsl"), ShaderStage.Mesh))
			.SetPixelShader(new ShaderModule(Embed.GetString("Shaders/Standard/Prepass/PrepassPS.hlsl"), ShaderStage.Pixel))
			.AsRootConstant(0, 1)
			.SetDepthMode(DepthMode.GreaterEqual, true, true)
			.SetCullMode(CullMode.CCW)
			.SetRTFormat(0, Vortice.DXGI.Format.R32G32_UInt)
			.Compile().Result;

		// Indirect command signature for depth pass.
		commandSignature = new CommandSignature()
			.AddConstantArg(0, visPSO)
			.AddDispatchMeshArg()
			.Compile();

		commandBuffer = new GraphicsBuffer(commandSignature.Stride * Scene.MaxInstances, commandSignature.Stride, hasCounter: true);
	}

	public override void Run(CommandList list)
	{
		// Perform culling/build indirect draw commands
		BuildIndirectCommands(list);

		// Switch to prepass PSO
		list.SetPipelineState(visPSO);
		list.SetPipelineSRV(0, 1, Mesh.VertexBuffer);
		list.SetPipelineSRV(1, 1, Mesh.IndexBuffer);
		list.SetPipelineSRV(2, 1, Mesh.MeshletBuffer);
		list.SetPipelineSRV(3, 1, Mesh.MeshBuffer);
		list.SetPipelineSRV(4, 1, Camera.Scene.TransformBuffer);
		list.SetPipelineSRV(5, 1, Camera.Scene.InstanceBuffer);
		list.SetPipelineCBV(0, 1, RP.ViewCB);

		// Output to vis/depth buffers
		list.SetRenderTarget(RP.VisBuffer, RP.DepthBuffer);

		// Indirect dispatch
		list.BarrierUAV(commandBuffer);
		list.ExecuteIndirect(commandSignature, commandBuffer, (int)Camera.Scene.InstanceBuffer.NumAllocations);
	}

	private void BuildIndirectCommands(CommandList list)
	{
		// Reset command count
		list.ResetCounter(commandBuffer);

		// Switch to indirect culling PSO
		list.SetPipelineState(cullPSO);
		list.SetPipelineSRV(3, 1, Mesh.MeshBuffer);
		list.SetPipelineSRV(5, 1, Camera.Scene.InstanceBuffer);
		list.SetPipelineUAV(0, 0, commandBuffer);

		// Compute dispatch
		if (Camera.Scene.InstanceBuffer.NumAllocations > 0)
		{
			list.Dispatch((int)(Camera.Scene.InstanceBuffer.LastOffset + 1));
		}
	}
}