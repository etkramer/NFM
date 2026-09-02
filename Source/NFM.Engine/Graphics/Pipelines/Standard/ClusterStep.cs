using NFM.GPU;

namespace NFM.Graphics;

/// <summary>
/// The froxel grid <see cref="ClusterStep"/> fills in, walked by every pass that shades.
/// </summary>
record ClusterBuffers(BufferHandle Counts, BufferHandle Offsets, BufferHandle Lights);

/// <summary>
/// Bins lights into a froxel grid, one contiguous run of light indices per cluster.
/// </summary>
class ClusterStep : ViewPass
{
	public const int TileSize = 16;
	public const int SliceCount = 32;

	// The range slices are distributed over, independent of the view's own near/far planes.
	public const float NearDepth = 0.1f;
	public const float FarDepth = 512;

	// Light index pool, shared by every cluster.
	private const int LightPool = 4 * 1024 * 1024;

	private const int ScanGroupSize = 256;
	private const int ClearGroupSize = 64;

	private const int TargetGroups = 4096;
	private const int MaxGroupsPerLight = 1024;

	private static PipelineState? clearPSO;
	private static PipelineState? countPSO;
	private static PipelineState? scanPSO;
	private static PipelineState? scanBlocksPSO;
	private static PipelineState? scanAddPSO;
	private static PipelineState? scatterPSO;

	private RawBuffer? cursors;
	private RawBuffer? blockSums;

	private int clusterCount;
	private int blockCount;

	private readonly StandardResources resources;

	public ClusterStep(StandardResources resources)
	{
		this.resources = resources;
	}

	private RawBuffer Cursors => Guard.NotNull(cursors);
	private RawBuffer BlockSums => Guard.NotNull(blockSums);

	public static Vector3i DimensionsFor(Vector2i size)
	{
		return new(MathHelper.IntCeiling(size.X / (float)TileSize), MathHelper.IntCeiling(size.Y / (float)TileSize), SliceCount);
	}

	public static int CountFor(Vector2i size)
	{
		Vector3i dims = DimensionsFor(size);
		return dims.X * dims.Y * dims.Z;
	}

	/// <summary>
	/// Declares the buffers this pass fills in, sized for a view of a given resolution.
	/// </summary>
	public static ClusterBuffers Declare(RenderGraph graph, Vector2i size)
	{
		nint clusterBytes = (nint)CountFor(size) * sizeof(uint);

		return new ClusterBuffers(
			graph.CreateBuffer("Cluster Counts", new(clusterBytes, sizeof(uint))),
			graph.CreateBuffer("Cluster Offsets", new(clusterBytes, sizeof(uint))),
			graph.CreateBuffer("Cluster Lights", new((nint)LightPool * sizeof(uint), sizeof(uint))));
	}

	public override void Setup(RenderGraphBuilder builder)
	{
		builder.Write(resources.Clusters.Counts, resources.Clusters.Offsets, resources.Clusters.Lights);
	}

	public override void Init(RenderGraph graph)
	{
		clearPSO ??= new PipelineState()
			.SetComputeShader(new ShaderModule(Embed.GetString("Shaders/Standard/Cluster/ClusterClearCS.hlsl"), ShaderStage.Compute))
			.AsRootConstant(0, 1)
			.Compile().Result;

		countPSO ??= new PipelineState()
			.SetComputeShader(new ShaderModule(Embed.GetString("Shaders/Standard/Cluster/ClusterCountCS.hlsl"), ShaderStage.Compute))
			.AsRootConstant(0, 3)
			.Compile().Result;

		scatterPSO ??= new PipelineState()
			.SetComputeShader(new ShaderModule(Embed.GetString("Shaders/Standard/Cluster/ClusterScatterCS.hlsl"), ShaderStage.Compute))
			.AsRootConstant(0, 3)
			.Compile().Result;

		scanPSO ??= new PipelineState()
			.SetComputeShader(new ShaderModule(Embed.GetString("Shaders/Common/Scan/ScanCS.hlsl"), ShaderStage.Compute))
			.AsRootConstant(0, 1)
			.Compile().Result;

		scanBlocksPSO ??= new PipelineState()
			.SetComputeShader(new ShaderModule(Embed.GetString("Shaders/Common/Scan/ScanBlocksCS.hlsl"), ShaderStage.Compute))
			.AsRootConstant(0, 1)
			.Compile().Result;

		scanAddPSO ??= new PipelineState()
			.SetComputeShader(new ShaderModule(Embed.GetString("Shaders/Common/Scan/ScanAddCS.hlsl"), ShaderStage.Compute))
			.AsRootConstant(0, 1)
			.Compile().Result;

		clusterCount = CountFor(graph.Get(resources.DepthBuffer).Size);
		blockCount = MathHelper.IntCeiling(clusterCount / (float)ScanGroupSize);

		cursors = new RawBuffer((nint)clusterCount * sizeof(uint), sizeof(uint)) { Name = "Cluster Cursors" };
		blockSums = new RawBuffer((nint)blockCount * sizeof(uint), sizeof(uint)) { Name = "Cluster Block Sums" };
	}

	public override void Run(in ViewPassContext ctx)
	{
		var list = ctx.List;
		var scene = ctx.RenderScene;

		Vector2i size = ctx.Get(resources.DepthBuffer).Size;
		int lightCount = scene.LightCount;

		RawBuffer counts = ctx.Get(resources.Clusters.Counts);
		RawBuffer offsets = ctx.Get(resources.Clusters.Offsets);
		RawBuffer lights = ctx.Get(resources.Clusters.Lights);

		list.SetPipelineState(Guard.NotNull(clearPSO));
		list.SetPipelineUAV(0, 0, counts);
		list.SetPipelineUAV(1, 0, Cursors);
		list.SetPipelineConstants(0, 0, clusterCount);
		list.DispatchThreads(clusterCount, ClearGroupSize);

		list.BarrierUAV(counts, Cursors);

		// Spread each light over enough groups to keep the dispatch wide.
		int groupsPerLight = Math.Clamp(TargetGroups / Math.Max(lightCount, 1), 1, MaxGroupsPerLight);

		if (lightCount > 0)
		{
			list.SetPipelineState(Guard.NotNull(countPSO));
			list.SetPipelineUAV(0, 0, counts);
			list.SetPipelineSRV(6, 1, scene.LightBuffer);
			list.SetPipelineCBV(0, 1, ctx.ViewCB);
			list.SetPipelineConstants(0, 0, size.X, size.Y, groupsPerLight);
			list.Dispatch(groupsPerLight, lightCount);

			list.BarrierUAV(counts);
		}

		Scan(list, counts, offsets);

		if (lightCount > 0)
		{
			list.SetPipelineState(Guard.NotNull(scatterPSO));
			list.SetPipelineSRV(0, 0, offsets);
			list.SetPipelineUAV(0, 0, Cursors);
			list.SetPipelineUAV(1, 0, lights);
			list.SetPipelineSRV(6, 1, scene.LightBuffer);
			list.SetPipelineCBV(0, 1, ctx.ViewCB);
			list.SetPipelineConstants(0, 0, size.X, size.Y, groupsPerLight);
			list.Dispatch(groupsPerLight, lightCount);

			list.BarrierUAV(lights);
		}
	}

	/// <summary>
	/// Turns per-cluster counts into each cluster's start in the shared pool.
	/// </summary>
	private void Scan(CommandList list, RawBuffer counts, RawBuffer offsets)
	{
		var sums = BlockSums;

		list.BeginEvent("Scan clusters");

		list.SetPipelineState(Guard.NotNull(scanPSO));
		list.SetPipelineSRV(0, 0, counts);
		list.SetPipelineUAV(0, 0, offsets);
		list.SetPipelineUAV(1, 0, sums);
		list.SetPipelineConstants(0, 0, clusterCount);
		list.DispatchThreads(clusterCount, ScanGroupSize);

		list.BarrierUAV(offsets, sums);

		list.SetPipelineState(Guard.NotNull(scanBlocksPSO));
		list.SetPipelineUAV(0, 0, sums);
		list.SetPipelineConstants(0, 0, blockCount);
		list.Dispatch(1);

		list.BarrierUAV(sums);

		list.SetPipelineState(Guard.NotNull(scanAddPSO));
		list.SetPipelineSRV(0, 0, sums);
		list.SetPipelineUAV(0, 0, offsets);
		list.SetPipelineConstants(0, 0, clusterCount);
		list.DispatchThreads(clusterCount, ScanGroupSize);

		list.BarrierUAV(offsets);
		list.EndEvent();
	}

	public override void Dispose()
	{
		cursors?.Dispose();
		blockSums?.Dispose();

		base.Dispose();
	}
}
