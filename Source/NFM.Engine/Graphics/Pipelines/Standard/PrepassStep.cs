using NFM.GPU;

namespace NFM.Graphics;

class PrepassStep : ViewPass
{
	private static PipelineState? cullPSO;
	private static PipelineState? visPSO;

	private static RawBuffer? commandBuffer;
	private static CommandSignature? commandSignature;

	private readonly StandardResources resources;

	public PrepassStep(StandardResources resources)
	{
		this.resources = resources;
	}

	public override void Setup(RenderGraphBuilder builder)
	{
		builder.Write(resources.VisBuffer, resources.DepthBuffer);
	}

	public override void Init(RenderGraph graph)
	{
		// Compile indirect compute program.
		cullPSO ??= new PipelineState()
			.SetComputeShader(new ShaderModule(Embed.GetString("Shaders/Standard/CullCS.hlsl"), ShaderStage.Compute))
			.Compile().Result;

		// Compile depth prepass program.
		visPSO ??= new PipelineState()
			.SetVertexShader(new ShaderModule(Embed.GetString("Shaders/Standard/Prepass/BaseVS.hlsl"), ShaderStage.Vertex))
			.SetPixelShader(new ShaderModule(Embed.GetString("Shaders/Standard/Prepass/PrepassPS.hlsl"), ShaderStage.Pixel))
			.AsRootConstant(0, 1)
			.SetDepthMode(DepthMode.GreaterEqual, true, true)
			.SetCullMode(CullMode.CCW)
			.SetRTFormat(0, Vortice.DXGI.Format.R32G32_UInt)
			.Compile().Result;

		// Indirect command signature for depth pass.
		commandSignature ??= new CommandSignature()
			.AddConstantArg(0, visPSO)
			.AddDrawIndexedArg()
			.Compile();

		commandBuffer ??= new RawBuffer(commandSignature.Stride * RenderScene.MaxInstances, commandSignature.Stride, hasCounter: true);
	}

	public override void Run(in ViewPassContext ctx)
	{
        Guard.NotNull(visPSO);
        Guard.NotNull(commandBuffer);
        Guard.NotNull(commandSignature);

		var list = ctx.List;
		var scene = ctx.RenderScene;

		// Perform culling/build indirect draw commands
		BuildIndirectCommands(list, scene);

		// Switch to prepass PSO
		list.SetPipelineState(visPSO);
		list.SetPipelineSRV(0, 1, RenderMesh.VertexBuffer);
		list.SetPipelineSRV(3, 1, RenderMesh.MeshBuffer);
		list.SetPipelineSRV(4, 1, scene.TransformBuffer);
		list.SetPipelineSRV(5, 1, scene.InstanceBuffer);
		list.SetPipelineCBV(0, 1, ctx.ViewCB);

		// Output to vis/depth buffers
		list.SetRenderTarget(ctx.Get(resources.VisBuffer), ctx.Get(resources.DepthBuffer));

		list.SetIndexBuffer(RenderMesh.IndexBuffer);

		// Indirect dispatch
		if (scene.InstanceBuffer.NumAllocations > 0)
		{
			list.BarrierUAV(commandBuffer);
			list.ExecuteIndirect(commandSignature, commandBuffer, (int)scene.InstanceBuffer.NumAllocations);
		}
	}

	private void BuildIndirectCommands(CommandList list, RenderScene scene)
	{
		// Reset command count
		list.ResetCounter(commandBuffer!);

		// Switch to indirect culling PSO
		list.SetPipelineState(cullPSO!);
		list.SetPipelineSRV(3, 1, RenderMesh.MeshBuffer);
		list.SetPipelineSRV(5, 1, scene.InstanceBuffer);
		list.SetPipelineUAV(0, 0, commandBuffer!);

		// Compute dispatch
		if (scene.InstanceBuffer.NumAllocations > 0)
		{
			list.Dispatch((int)(scene.InstanceBuffer.LastOffset + 1));
		}
	}
}
