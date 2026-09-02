using NFM.GPU;

namespace NFM.Graphics;

/// <summary>
/// Bins lights into a froxel grid, so shading and shadow tracing only ever visit the lights that
/// can reach a pixel. Each light scatters into the clusters its bounds cover and a prefix sum packs
/// the results into one contiguous run per cluster, leaving nothing capped but the pool as a whole.
/// </summary>
class ClusterStep : ViewPass
{
	public const int TileSize = 16;
	public const int SliceCount = 32;

	// The range slices are distributed over. Unrelated to the view's own near plane, which is far
	// too close to spend slices on, and to its far plane, which is at infinity.
	public const float NearDepth = 0.1f;
	public const float FarDepth = 512;

	// Shared by every cluster; a scene dense enough to exhaust it drops its last few assignments.
	private const int LightPool = 4 * 1024 * 1024;

	private const int ScanGroupSize = 256;
	private const int LightGroupSize = 64;

	// Roughly how many groups it takes to fill a modern GPU, and the ceiling one light may claim.
	private const int TargetGroups = 4096;
	private const int MaxGroupsPerLight = 1024;

	private static PipelineState? clearPSO;
	private static PipelineState? countPSO;
	private static PipelineState? scanPSO;
	private static PipelineState? scanBlocksPSO;
	private static PipelineState? scanAddPSO;
	private static PipelineState? scatterPSO;

	private RawBuffer? counts;
	private RawBuffer? offsets;
	private RawBuffer? cursors;
	private RawBuffer? blockSums;
	private RawBuffer? lights;

	private int clusterCount;
	private int blockCount;

	private readonly StandardResources resources;

	public ClusterStep(StandardResources resources)
	{
		this.resources = resources;
	}

	public RawBuffer Counts => Guard.NotNull(counts);
	public RawBuffer Offsets => Guard.NotNull(offsets);
	public RawBuffer Lights => Guard.NotNull(lights);

	public static Vector3i DimensionsFor(Vector2i size)
	{
		return new(MathHelper.IntCeiling(size.X / (float)TileSize), MathHelper.IntCeiling(size.Y / (float)TileSize), SliceCount);
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

		Vector3i dims = DimensionsFor(graph.Get(resources.DepthBuffer).Size);

		clusterCount = dims.X * dims.Y * dims.Z;
		blockCount = MathHelper.IntCeiling(clusterCount / (float)ScanGroupSize);

		counts = new RawBuffer((nint)clusterCount * sizeof(uint), sizeof(uint)) { Name = "Cluster Counts" };
		offsets = new RawBuffer((nint)clusterCount * sizeof(uint), sizeof(uint)) { Name = "Cluster Offsets" };
		cursors = new RawBuffer((nint)clusterCount * sizeof(uint), sizeof(uint)) { Name = "Cluster Cursors" };
		blockSums = new RawBuffer((nint)blockCount * sizeof(uint), sizeof(uint)) { Name = "Cluster Block Sums" };
		lights = new RawBuffer((nint)LightPool * sizeof(uint), sizeof(uint)) { Name = "Cluster Lights" };
	}

	public override void Run(in ViewPassContext ctx)
	{
		var list = ctx.List;
		var scene = ctx.RenderScene;

		Vector2i size = ctx.Get(resources.DepthBuffer).Size;
		int lightCount = scene.LightCount;

		list.SetPipelineState(Guard.NotNull(clearPSO));
		list.SetPipelineUAV(0, 0, Counts);
		list.SetPipelineUAV(1, 0, Guard.NotNull(cursors));
		list.SetPipelineConstants(0, 0, clusterCount);
		list.DispatchThreads(clusterCount, LightGroupSize);

		list.BarrierUAV(Counts, Guard.NotNull(cursors));

		// Few large lights would otherwise leave most of the GPU idle, so each one is spread over
		// however many groups it takes to keep the dispatch wide.
		int groupsPerLight = Math.Clamp(TargetGroups / Math.Max(lightCount, 1), 1, MaxGroupsPerLight);

		if (lightCount > 0)
		{
			list.SetPipelineState(Guard.NotNull(countPSO));
			list.SetPipelineUAV(0, 0, Counts);
			list.SetPipelineSRV(6, 1, scene.LightBuffer);
			list.SetPipelineCBV(0, 1, ctx.ViewCB);
			list.SetPipelineConstants(0, 0, size.X, size.Y, groupsPerLight);
			list.Dispatch(groupsPerLight, lightCount);

			list.BarrierUAV(Counts);
		}

		Scan(list);

		if (lightCount > 0)
		{
			list.SetPipelineState(Guard.NotNull(scatterPSO));
			list.SetPipelineSRV(0, 0, Offsets);
			list.SetPipelineUAV(0, 0, Guard.NotNull(cursors));
			list.SetPipelineUAV(1, 0, Lights);
			list.SetPipelineSRV(6, 1, scene.LightBuffer);
			list.SetPipelineCBV(0, 1, ctx.ViewCB);
			list.SetPipelineConstants(0, 0, size.X, size.Y, groupsPerLight);
			list.Dispatch(groupsPerLight, lightCount);

			list.BarrierUAV(Lights);
		}
	}

	/// <summary>
	/// Turns per-cluster counts into each cluster's start in the shared pool.
	/// </summary>
	private void Scan(CommandList list)
	{
		var sums = Guard.NotNull(blockSums);

		list.BeginEvent("Scan clusters");

		list.SetPipelineState(Guard.NotNull(scanPSO));
		list.SetPipelineSRV(0, 0, Counts);
		list.SetPipelineUAV(0, 0, Offsets);
		list.SetPipelineUAV(1, 0, sums);
		list.SetPipelineConstants(0, 0, clusterCount);
		list.DispatchThreads(clusterCount, ScanGroupSize);

		list.BarrierUAV(Offsets, sums);

		list.SetPipelineState(Guard.NotNull(scanBlocksPSO));
		list.SetPipelineUAV(0, 0, sums);
		list.SetPipelineConstants(0, 0, blockCount);
		list.Dispatch(1);

		list.BarrierUAV(sums);

		list.SetPipelineState(Guard.NotNull(scanAddPSO));
		list.SetPipelineSRV(0, 0, sums);
		list.SetPipelineUAV(0, 0, Offsets);
		list.SetPipelineConstants(0, 0, clusterCount);
		list.DispatchThreads(clusterCount, ScanGroupSize);

		list.BarrierUAV(Offsets);
		list.EndEvent();
	}

	public override void Dispose()
	{
		counts?.Dispose();
		offsets?.Dispose();
		cursors?.Dispose();
		blockSums?.Dispose();
		lights?.Dispose();

		base.Dispose();
	}
}
