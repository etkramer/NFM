using NFM.GPU;
using Vortice.Direct3D12;

namespace NFM.Graphics;

/// <summary>
/// Builds and refits the scene's acceleration structures.
/// </summary>
class BVHStep : ScenePass
{
	private const int ScratchSize = 64 * 1024 * 1024;

	private static PipelineState? instancePSO;

	private readonly ScratchAllocator scratch = new(ScratchSize);

	public override void Init()
	{
		instancePSO ??= new PipelineState()
			.SetComputeShader(new ShaderModule(Embed.GetString("Shaders/Common/BuildInstancesCS.hlsl"), ShaderStage.Compute))
			.AsRootConstant(0, 1)
			.Compile().Result;
	}

	public override void Run(in ScenePassContext ctx)
	{
		var list = ctx.List;
		var scene = ctx.RenderScene;

		bool hasBuilds = RenderMesh.PendingBuilds.Count > 0 || scene.DeformedNodes.Count > 0;

		if (!scene.TakeStructuresDirty() && !hasBuilds)
		{
			return;
		}

		scratch.Reset(list);

		if (hasBuilds)
		{
			list.RequestState(RenderMesh.VertexBuffer, ResourceStates.NonPixelShaderResource);
			list.RequestState(RenderMesh.IndexBuffer, ResourceStates.NonPixelShaderResource);

			foreach (var mesh in RenderMesh.PendingBuilds)
			{
				mesh.BLAS.Build(list, scratch.Allocate(list, mesh.BLAS.ScratchSize));
			}

			foreach (var node in scene.DeformedNodes)
			{
				foreach (var skin in node.SkinHandles.Values)
				{
					skin.BLAS.Build(list, scratch.Allocate(list, skin.BLAS.ScratchSize));
				}
			}

			RenderMesh.PendingBuilds.Clear();
			scene.DeformedNodes.Clear();
		}

		int instanceCount = scene.InstanceCount;
		scene.TLAS.EnsureCapacity(instanceCount);

		if (instanceCount > 0)
		{
			list.SetPipelineState(Guard.NotNull(instancePSO));
			list.SetPipelineUAV(0, 0, scene.TLAS.Instances);
			list.SetPipelineSRV(4, 1, scene.TransformBuffer);
			list.SetPipelineSRV(5, 1, scene.InstanceBuffer);
			list.SetPipelineConstants(0, 0, instanceCount);

			list.DispatchThreads(instanceCount, 64);
		}

		list.BarrierUAV();
		scene.TLAS.Build(list, instanceCount, scratch.Allocate(list, scene.TLAS.ScratchSize));
		list.BarrierUAV();
	}

	public override void Dispose()
	{
		scratch.Dispose();

		base.Dispose();
	}
}
