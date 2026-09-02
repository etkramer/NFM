using NFM.GPU;
using NFM.World;

namespace NFM.Graphics;

class SkinningStep : ScenePass
{
	private static PipelineState? skinPSO;

	public override void Init()
	{
		skinPSO ??= new PipelineState()
			.SetComputeShader(new ShaderModule(Embed.GetString("Shaders/Common/SkinCS.hlsl"), ShaderStage.Compute))
			.AsRootConstant(0, 5)
			.Compile().Result;
	}

	public override void Run(in ScenePassContext ctx)
	{
		var scene = ctx.RenderScene;
		if (scene.DeformedNodes.Count == 0)
		{
			return;
		}

		var list = ctx.List;

		list.SetPipelineState(Guard.NotNull(skinPSO));
		list.SetPipelineSRV(2, 1, RenderMesh.WeightBuffer);
		list.SetPipelineSRV(7, 1, scene.BoneBuffer);
		list.SetPipelineUAV(0, 0, RenderMesh.VertexBuffer);

		foreach (var node in scene.DeformedNodes)
		{
			int boneOffset = (int)Guard.NotNull(node.BoneHandle).Offset;

			foreach (var (mesh, skin) in node.SkinHandles)
			{
				var source = Guard.NotNull(mesh.RenderData);
				int vertexCount = (int)source.VertexHandle.Size;

				list.SetPipelineConstants(0, 0,
					(int)source.VertexHandle.Offset,
					(int)skin.Vertices.Offset,
					(int)Guard.NotNull(source.WeightHandle).Offset,
					boneOffset,
					vertexCount);

				list.DispatchThreads(vertexCount, 64);
			}
		}

		list.BarrierUAV(RenderMesh.VertexBuffer);
	}
}
