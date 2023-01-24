using System;
using NFM.GPU;
using NFM.Resources;
using NFM.World;
using Vortice.DXGI;

namespace NFM.Graphics;

public class MaterialStep : CameraStep<StandardRenderPipeline>
{
	private GraphicsBuffer commandBuffer;
	private PipelineState cullPSO;

	public override void Init()
	{
		// Compile indirect compute program.
		cullPSO = new PipelineState()
			.UseIncludes(typeof(Engine).Assembly)
			.SetComputeShader(Embed.GetString("Shaders/Standard/Prepass/CullCS.hlsl", typeof(Engine).Assembly), "CullCS")
			.AsRootConstant(0, 1)
			.Compile().Result;

		const int commandStride = 16;
		commandBuffer = new GraphicsBuffer(commandStride * Scene.MaxInstances, commandStride, hasCounter: true);
	}

	public override void Run(CommandList list)
	{
		// Loop through materials to shade.
		foreach (var permutation in ShaderPermutation.All)
		{
			int shaderID = permutation.ShaderID;

			// Build indirect draw commands for this shader ID.
			BuildDraws(shaderID, list);

			list.BeginEvent($"Draw shader {shaderID}");

			// Switch to material program.
			list.SetPipelineState(permutation.MaterialPSO);

			// Set and reset render targets.
			list.SetRenderTarget(RP.ColorTarget, RP.DepthBuffer);

			// Bind program SRVs.
			list.SetPipelineSRV(0, 1, Mesh.VertBuffer);
			list.SetPipelineSRV(1, 1, Mesh.PrimBuffer);
			list.SetPipelineSRV(2, 1, Mesh.MeshletBuffer);
			list.SetPipelineSRV(3, 1, Mesh.MeshBuffer);
			list.SetPipelineSRV(4, 1, Camera.Scene.TransformBuffer);
			list.SetPipelineSRV(5, 1, Camera.Scene.InstanceBuffer);
			list.SetPipelineSRV(0, 0, MaterialInstance.MaterialBuffer);

			// Bind program CBVs.
			list.SetPipelineCBV(0, 1, RP.ViewCB);

			list.ExecuteIndirect(permutation.MaterialSignature, commandBuffer, (int)Camera.Scene.InstanceBuffer.NumAllocations);
			list.EndEvent();
		}
	}

	private void BuildDraws(int shaderID, CommandList list)
	{
		// Reset command count.
		list.ResetCounter(commandBuffer);

		// Switch to culling program (compute).
		list.SetPipelineState(cullPSO);

		// Set SRV inputs.
		list.SetPipelineSRV(0, 0, MaterialInstance.MaterialBuffer);
		list.SetPipelineSRV(3, 1, Mesh.MeshBuffer);
		list.SetPipelineSRV(5, 1, Camera.Scene.InstanceBuffer);

		// Set UAV outputs.
		list.SetPipelineUAV(0, 0, commandBuffer);

		// Build for chosen shader.
		list.SetPipelineConstants(0, 0, shaderID);

		// Dispatch compute shader.
		if (Camera.Scene.InstanceBuffer.NumAllocations > 0)
		{
			list.Dispatch((int)Camera.Scene.InstanceBuffer.LastOffset + 1);
		}

		list.BarrierUAV(commandBuffer);
	}
}