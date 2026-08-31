using NFM.GPU;

namespace NFM.Graphics;

/// <summary>
/// Samples the visbuffer under the cursor every frame, so the hovered object is always known without a stall.
/// </summary>
class PickingStep : ViewPass
{
	private const int ResultSize = 8; // uint2

	private static PipelineState? pickPSO;

	private RawBuffer? resultBuffer;
	private ReadbackBuffer? readbackBuffer;

	// One readback slot per frame in flight, so we never read a slot the GPU could still be writing.
	// Holds the frame each slot was written on, or MaxValue if it holds nothing.
	private ulong[] slotFrames = [];

	private readonly StandardResources resources;

	public PickingStep(StandardResources resources)
	{
		this.resources = resources;
	}

	public override void Setup(RenderGraphBuilder builder)
	{
		builder.Read(resources.VisBuffer, resources.DepthBuffer);
	}

	public override void Init(RenderGraph graph)
	{
		pickPSO ??= new PipelineState()
			.SetComputeShader(new ShaderModule(Embed.GetString("Shaders/Standard/PickCS.hlsl"), ShaderStage.Compute))
			.AsRootConstant(0, 2)
			.Compile().Result;

		resultBuffer = new RawBuffer(ResultSize, ResultSize) { Name = "Pick Result" };
		readbackBuffer = new ReadbackBuffer(ResultSize * D3DContext.RenderLatency) { Name = "Pick Readback" };

		slotFrames = new ulong[D3DContext.RenderLatency];
		Array.Fill(slotFrames, ulong.MaxValue);
	}

	public override void Run(in ViewPassContext ctx)
	{
		var camera = ctx.Camera;
		var visBuffer = ctx.Get(resources.VisBuffer);

		Vector2i size = visBuffer.Size;
		if (camera.PickCoords is not Vector2i coords || coords.X < 0 || coords.Y < 0 || coords.X >= size.X || coords.Y >= size.Y)
		{
			// Nothing to sample - drop anything still in flight so it can't later be read as current.
			Array.Fill(slotFrames, ulong.MaxValue);
			camera.HoveredInstance = -1;
			return;
		}

		ResolveRetiredSlot(camera);

		var list = ctx.List;

		list.SetPipelineState(Guard.NotNull(pickPSO));
		list.SetPipelineSRV(0, 0, visBuffer);
		list.SetPipelineSRV(1, 0, ctx.Get(resources.DepthBuffer));
		list.SetPipelineUAV(0, 0, Guard.NotNull(resultBuffer));
		list.SetPipelineConstants(0, 0, coords.X, coords.Y);
		list.Dispatch(1);

		// The result buffer is reused every frame, but the copies queue behind each other, so each slot keeps its own frame.
		int slot = (int)(Metrics.FrameCount % (ulong)slotFrames.Length);

		list.BarrierUAV(resultBuffer);
		list.CopyToReadback(resultBuffer, Guard.NotNull(readbackBuffer), ResultSize, destOffset: slot * ResultSize);

		slotFrames[slot] = Metrics.FrameCount;
	}

	private void ResolveRetiredSlot(World.CameraNode camera)
	{
		ulong completed = D3DContext.CompletedFrame;

		// Take the most recent slot the GPU has finished with.
		int newest = -1;
		for (int i = 0; i < slotFrames.Length; i++)
		{
			if (slotFrames[i] <= completed && (newest < 0 || slotFrames[i] > slotFrames[newest]))
			{
				newest = i;
			}
		}

		if (newest < 0)
		{
			return;
		}

		ReadOnlySpan<uint> result = Guard.NotNull(readbackBuffer).Read<uint>(2, newest * ResultSize);
		camera.HoveredInstance = result[0] == 0 ? -1 : (int)result[1];
	}

	public override void Dispose()
	{
		resultBuffer?.Dispose();
		readbackBuffer?.Dispose();

		base.Dispose();
	}
}
