using NFM.GPU;
using Vortice.Direct3D12;

namespace NFM.Graphics;

/// <summary>
/// Keeps the scene traceable: builds structures for newly loaded geometry, refits whatever the
/// skinning pass deformed, and rebuilds the top-level structure over every live instance.
/// </summary>
class BVHStep : ScenePass
{
	// Holds a frame's worth of builds. Overflowing costs a barrier, not correctness.
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

		scratch.Reset(list);

		// Builds source their triangles straight out of the shared geometry buffers.
		list.RequestState(RenderMesh.VertexBuffer, ResourceStates.NonPixelShaderResource);
		list.RequestState(RenderMesh.IndexBuffer, ResourceStates.NonPixelShaderResource);

		foreach (var mesh in RenderMesh.PendingBuilds)
		{
			mesh.BLAS.Build(list, scratch.Allocate(list, mesh.BLAS.ScratchSize));
		}

		RenderMesh.PendingBuilds.Clear();

		foreach (var node in scene.SkinnedNodes)
		{
			foreach (var skin in node.SkinHandles.Values)
			{
				skin.BLAS.Build(list, scratch.Allocate(list, skin.BLAS.ScratchSize));
			}
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

		// Nothing can reference a bottom-level structure until its build has landed.
		list.BarrierUAV();
		scene.TLAS.Build(list, instanceCount, scratch.Allocate(list, scene.TLAS.ScratchSize));
		list.BarrierUAV();
	}

	public override void Dispose()
	{
		scratch.Dispose();
	}
}
