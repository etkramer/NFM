using NFM.GPU;

namespace NFM.Graphics;

/// <summary>
/// Resolves the visbuffer into g-buffers. Pixels are first bucketed by shader stack, so each material
/// shader runs an indirect dispatch over exactly its own pixels rather than the whole screen.
/// </summary>
class MaterialStep : ViewPass
{
	private const int MaxStacks = RenderMaterial.MaxStacks;

	private static PipelineState? countPSO;
	private static PipelineState? scanPSO;
	private static PipelineState? scatterPSO;
	private static CommandSignature? dispatchSignature;

	private RawBuffer? binCounts;
	private RawBuffer? binOffsets;
	private RawBuffer? binCursors;
	private RawBuffer? binDispatchArgs;
	private RawBuffer? binPixels;

	private readonly StandardResources resources;

	public MaterialStep(StandardResources resources)
	{
		this.resources = resources;
	}

	public override void Setup(RenderGraphBuilder builder)
	{
		builder.Read(resources.VisBuffer, resources.DepthBuffer);
		builder.Write(resources.MatBuffer0, resources.MatBuffer1, resources.MatBuffer2, resources.MatBuffer3);
	}

	public override void Init(RenderGraph graph)
	{
		// Request a permutation for each shader combination
		RenderMaterial.RequestPermutation<MaterialShaderPermutation>();

		countPSO ??= new PipelineState()
			.SetComputeShader(new ShaderModule(Embed.GetString("Shaders/Standard/Bin/BinCountCS.hlsl"), ShaderStage.Compute))
			.Compile().Result;

		scanPSO ??= new PipelineState()
			.SetComputeShader(new ShaderModule(Embed.GetString("Shaders/Standard/Bin/BinScanCS.hlsl"), ShaderStage.Compute))
			.Compile().Result;

		scatterPSO ??= new PipelineState()
			.SetComputeShader(new ShaderModule(Embed.GetString("Shaders/Standard/Bin/BinScatterCS.hlsl"), ShaderStage.Compute))
			.Compile().Result;

		dispatchSignature ??= new CommandSignature()
			.AddDispatchArg()
			.Compile();

		Vector2i size = graph.Get(resources.VisBuffer).Size;

		binCounts = new RawBuffer(MaxStacks * sizeof(uint), sizeof(uint)) { Name = "Bin Counts" };
		binOffsets = new RawBuffer(MaxStacks * sizeof(uint), sizeof(uint)) { Name = "Bin Offsets" };
		binCursors = new RawBuffer(MaxStacks * sizeof(uint), sizeof(uint)) { Name = "Bin Cursors" };
		binDispatchArgs = new RawBuffer(MaxStacks * dispatchSignature.Stride, dispatchSignature.Stride) { Name = "Bin Dispatch Args" };
		binPixels = new RawBuffer((nint)size.X * size.Y * sizeof(uint), sizeof(uint)) { Name = "Bin Pixels" };
	}

	public override void Run(in ViewPassContext ctx)
	{
		if (!ShaderPermutation.All.TryGetValue(typeof(MaterialShaderPermutation), out var permutations))
		{
			return;
		}

		var list = ctx.List;
		var scene = ctx.RenderScene;

		var visBuffer = ctx.Get(resources.VisBuffer);
		var depthBuffer = ctx.Get(resources.DepthBuffer);

		BuildBins(list, scene, visBuffer, depthBuffer);

		foreach (MaterialShaderPermutation permutation in permutations)
		{
			list.BeginEvent($"Materials for StackID {permutation.StackID}");
			list.SetPipelineState(permutation.PSO!);

			// Bind inputs
			list.SetPipelineSRV(0, 0, visBuffer);
			list.SetPipelineSRV(1, 0, depthBuffer);
			list.SetPipelineSRV(2, 0, Guard.NotNull(binPixels));
			list.SetPipelineSRV(3, 0, Guard.NotNull(binCounts));
			list.SetPipelineSRV(4, 0, Guard.NotNull(binOffsets));
			list.SetPipelineSRV(0, 1, RenderMesh.VertexBuffer);
			list.SetPipelineSRV(1, 1, RenderMesh.IndexBuffer);
			list.SetPipelineSRV(3, 1, RenderMesh.MeshBuffer);
			list.SetPipelineSRV(4, 1, scene.TransformBuffer);
			list.SetPipelineSRV(5, 1, scene.InstanceBuffer);
			list.SetPipelineCBV(0, 1, ctx.ViewCB);
			list.SetPipelineSRV(0, 2, RenderMaterial.MaterialBuffer);

			// Material outputs
			list.SetPipelineUAV(0, 0, ctx.Get(resources.MatBuffer0));
			list.SetPipelineUAV(1, 0, ctx.Get(resources.MatBuffer1));
			list.SetPipelineUAV(2, 0, ctx.Get(resources.MatBuffer2));
			list.SetPipelineUAV(3, 0, ctx.Get(resources.MatBuffer3));

			// Dispatch over this shader's slice of the pixel list
			list.SetPipelineConstants(0, 0, permutation.StackID);
			list.ExecuteIndirect(Guard.NotNull(dispatchSignature), Guard.NotNull(binDispatchArgs), 1, permutation.StackID);
			list.EndEvent();
		}
	}

	private void BuildBins(CommandList list, RenderScene scene, Texture visBuffer, Texture depthBuffer)
	{
		Guard.NotNull(binCounts);
		Guard.NotNull(binOffsets);
		Guard.NotNull(binCursors);
		Guard.NotNull(binDispatchArgs);
		Guard.NotNull(binPixels);

		list.BeginEvent("Bin materials");

		// Counts are accumulated with atomics, so they have to start from zero.
		Span<uint> zeroes = stackalloc uint[MaxStacks];
		list.UploadBuffer(binCounts, (ReadOnlySpan<uint>)zeroes);

		// Count the pixels belonging to each shader stack.
		list.SetPipelineState(Guard.NotNull(countPSO));
		list.SetPipelineSRV(0, 0, visBuffer);
		list.SetPipelineSRV(1, 0, depthBuffer);
		list.SetPipelineSRV(5, 1, scene.InstanceBuffer);
		list.SetPipelineSRV(0, 2, RenderMaterial.MaterialBuffer);
		list.SetPipelineUAV(0, 0, binCounts);
		list.DispatchThreads(visBuffer.Width, 8, visBuffer.Height, 8);

		list.BarrierUAV(binCounts);

		// Turn counts into offsets, and into a dispatch arg per stack.
		list.SetPipelineState(Guard.NotNull(scanPSO));
		list.SetPipelineUAV(0, 0, binCounts);
		list.SetPipelineUAV(1, 0, binOffsets);
		list.SetPipelineUAV(2, 0, binCursors);
		list.SetPipelineUAV(3, 0, binDispatchArgs);
		list.Dispatch(1);

		list.BarrierUAV(binOffsets, binCursors, binDispatchArgs);

		// Scatter each pixel into its stack's slice of the list.
		list.SetPipelineState(Guard.NotNull(scatterPSO));
		list.SetPipelineSRV(0, 0, visBuffer);
		list.SetPipelineSRV(1, 0, depthBuffer);
		list.SetPipelineSRV(2, 0, binOffsets);
		list.SetPipelineSRV(5, 1, scene.InstanceBuffer);
		list.SetPipelineSRV(0, 2, RenderMaterial.MaterialBuffer);
		list.SetPipelineUAV(0, 0, binCursors);
		list.SetPipelineUAV(1, 0, binPixels);
		list.DispatchThreads(visBuffer.Width, 8, visBuffer.Height, 8);

		list.BarrierUAV(binPixels);
		list.EndEvent();
	}

	public override void Dispose()
	{
		binCounts?.Dispose();
		binOffsets?.Dispose();
		binCursors?.Dispose();
		binDispatchArgs?.Dispose();
		binPixels?.Dispose();

		base.Dispose();
	}
}

class MaterialShaderPermutation : ShaderPermutation
{
	private static readonly ShaderModule materialModule = new(Embed.GetString("Shaders/Standard/MaterialCS.hlsl"));

	public PipelineState? PSO { get; private set; }

	public override void Init(ShaderModule module)
	{
		PSO = new PipelineState()
			.SetComputeShader(materialModule.Link("main", ShaderStage.Compute, module))
			.AsRootConstant(0, 1)
			.Compile().Result;
	}

	public override void Dispose()
	{
		PSO?.Dispose();
	}
}
