﻿using System;
using NFM.GPU;
using NFM.Resources;
using NFM.World;

namespace NFM.Graphics;

public class MaterialStep : CameraStep<StandardRenderPipeline>
{
	private PipelineState previewPSO;

	public override void Init()
	{
		// Compile depth prepass program.
		previewPSO = new PipelineState()
			.UseIncludes(typeof(Engine).Assembly)
			.SetComputeShader(Embed.GetString("Shaders/Standard/MaterialCS.hlsl", typeof(Engine).Assembly), "MaterialCS")
			.Compile().Result;
	}

	public override void Run(CommandList list)
	{
		list.SetPipelineState(previewPSO);
		list.SetPipelineUAV(0, 0, RP.ColorTarget);
		list.SetPipelineSRV(0, 0, RP.VisBuffer);
		list.SetPipelineSRV(1, 0, RP.DepthBuffer);

		// Bind shared buffers.
		list.SetPipelineSRV(0, 1, Mesh.VertexBuffer);
		list.SetPipelineSRV(1, 1, Mesh.IndexBuffer);
		list.SetPipelineSRV(2, 1, Mesh.MeshletBuffer);
		list.SetPipelineSRV(3, 1, Mesh.MeshBuffer);
		list.SetPipelineSRV(4, 1, Camera.Scene.TransformBuffer);
		list.SetPipelineSRV(5, 1, Camera.Scene.InstanceBuffer);
		list.SetPipelineSRV(0, 2, MaterialInstance.MaterialBuffer);
		list.SetPipelineCBV(0, 1, RP.ViewCB);

		list.DispatchThreads(RP.ColorTarget.Width, 32, RP.ColorTarget.Height, 32);
	}
}