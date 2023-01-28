using System;
using NFM.GPU;
using NFM.Resources;

namespace NFM.Graphics;

public class MaterialStep : CameraStep<StandardRenderPipeline>
{
	public override void Init()
	{
		// Request a permutation for each shader combination
		MaterialInstance.RequestPermutation<MaterialShaderPermutation>();
	}

	public override void Run(CommandList list)
	{
		if (ShaderPermutation.All.TryGetValue(typeof(MaterialShaderPermutation), out var permutations))
		{
			foreach (MaterialShaderPermutation permutation in permutations)
			{
				list.BeginEvent($"Materials for StackID {permutation.StackID}");
				list.SetPipelineState(permutation.PSO);
				
				// Bind inputs
				list.SetPipelineSRV(0, 0, RP.VisBuffer);
				list.SetPipelineSRV(1, 0, RP.DepthBuffer);	
				list.SetPipelineSRV(0, 1, Mesh.VertexBuffer);
				list.SetPipelineSRV(1, 1, Mesh.IndexBuffer);
				list.SetPipelineSRV(2, 1, Mesh.MeshletBuffer);
				list.SetPipelineSRV(3, 1, Mesh.MeshBuffer);
				list.SetPipelineSRV(4, 1, Camera.Scene.TransformBuffer);
				list.SetPipelineSRV(5, 1, Camera.Scene.InstanceBuffer);
				list.SetPipelineCBV(0, 1, RP.ViewCB);
				list.SetPipelineSRV(0, 2, MaterialInstance.MaterialBuffer);

				// Material outputs
				list.SetPipelineUAV(0, 0, RP.MatBuffer0);
				list.SetPipelineUAV(1, 0, RP.MatBuffer1);
				list.SetPipelineUAV(2, 0, RP.MatBuffer2);

				// Dispatch material shader
				list.SetPipelineConstants(0, 0, permutation.StackID);
				list.DispatchThreads(RP.ColorTarget.Width, 32, RP.ColorTarget.Height, 32);
				list.EndEvent();
			}
		}
	}
}

public class MaterialShaderPermutation : ShaderPermutation
{
	private static ShaderModule materialModule = new ShaderModule(Embed.GetString("Shaders/Standard/MaterialCS.hlsl"));

	public PipelineState PSO { get; private set; }

	public override void Init(ShaderModule module)
	{
		PSO = new PipelineState()
			.SetComputeShader(materialModule.Link("main", ShaderStage.Compute, module))
			.AsRootConstant(0, 1)
			.Compile().Result;
	}

	public override void Dispose()
	{
		PSO.Dispose();
	}
}