using System;
using NFM.GPU;
using NFM.Resources;

namespace NFM.Graphics;

public class MaterialStep : CameraStep<StandardRenderPipeline>
{
	private PipelineState materialPSO;

	public override void Init()
	{
		// Request a permutation for each shader combination
		MaterialInstance.RequestPermutation<MaterialShaderPermutation>();

		// Compile material program
		materialPSO = new PipelineState()
			.SetComputeShader(new ShaderModule(Embed.GetString("Shaders/Standard/MaterialCS.hlsl"), ShaderStage.Compute))
			.Compile().Result;
	}

	public override void Run(CommandList list)
	{
		list.SetPipelineState(materialPSO);
		list.SetPipelineUAV(0, 0, RP.ColorTarget);
		list.SetPipelineSRV(0, 0, RP.VisBuffer);
		list.SetPipelineSRV(1, 0, RP.DepthBuffer);

		// Bind shared buffers
		list.SetPipelineSRV(0, 1, Mesh.VertexBuffer);
		list.SetPipelineSRV(1, 1, Mesh.IndexBuffer);
		list.SetPipelineSRV(2, 1, Mesh.MeshletBuffer);
		list.SetPipelineSRV(3, 1, Mesh.MeshBuffer);
		list.SetPipelineSRV(4, 1, Camera.Scene.TransformBuffer);
		list.SetPipelineSRV(5, 1, Camera.Scene.InstanceBuffer);
		list.SetPipelineCBV(0, 1, RP.ViewCB);
		list.SetPipelineSRV(0, 2, MaterialInstance.MaterialBuffer);

		list.DispatchThreads(RP.ColorTarget.Width, 32, RP.ColorTarget.Height, 32);
	}
}

public class MaterialShaderPermutation : ShaderPermutation
{
	public override void Init(ShaderModule module)
	{
		Console.WriteLine("Setup permutation");
	}

	public override void Dispose()
	{

	}
}