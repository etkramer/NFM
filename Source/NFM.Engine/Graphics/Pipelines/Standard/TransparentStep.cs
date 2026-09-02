using NFM.GPU;
using NFM.Resources;

namespace NFM.Graphics;

/// <summary>Draws blended geometry forward, back to front, over the lit scene.</summary>
class TransparentStep : ViewPass
{
	private readonly StandardResources resources;
	private readonly List<(float Distance, TransparentInstance Instance)> sorted = [];

	public TransparentStep(StandardResources resources)
	{
		this.resources = resources;
	}

	public override void Setup(RenderGraphBuilder builder)
	{
		builder.Read(resources.DepthBuffer);
		builder.Read(resources.Clusters.Counts, resources.Clusters.Offsets, resources.Clusters.Lights);
		builder.Write(resources.SceneColor);
	}

	public override void Init(RenderGraph graph)
	{
		RenderMaterial.RequestPermutation<TransparentShaderPermutation>();
	}

	public override void Run(in ViewPassContext ctx)
	{
		var list = ctx.List;
		var scene = ctx.RenderScene;

		Vector3 eye = ctx.Camera.WorldTransform.ExtractTranslation();

		sorted.Clear();
		foreach (var instance in scene.TransparentInstances)
		{
			sorted.Add(((instance.SortOrigin - eye).LengthSquared, instance));
		}

		if (sorted.Count == 0)
		{
			return;
		}

		// Farthest first, so each draw blends over everything already behind it.
		sorted.Sort((a, b) => b.Distance.CompareTo(a.Distance));

		list.BeginEvent("Transparency");

		list.SetRenderTarget(ctx.Get(resources.SceneColor), ctx.Get(resources.DepthBuffer));
		list.SetIndexBuffer(RenderMesh.IndexBuffer);

		PipelineState? boundPSO = null;

		foreach ((_, var instance) in sorted)
		{
			var permutation = instance.Material.GetPermutation<TransparentShaderPermutation>();
			if (permutation?.PSO is null || instance.Mesh.RenderData is null)
			{
				continue;
			}

			// Sorted draws aren't batched, so state can change between them.
			if (permutation.PSO != boundPSO)
			{
				list.SetPipelineState(permutation.PSO);
				boundPSO = permutation.PSO;

				list.SetPipelineSRV(0, 1, RenderMesh.VertexBuffer);
				list.SetPipelineSRV(3, 1, RenderMesh.MeshBuffer);
				list.SetPipelineSRV(4, 1, scene.TransformBuffer);
				list.SetPipelineSRV(5, 1, scene.InstanceBuffer);
				list.SetPipelineSRV(6, 1, scene.LightBuffer);
				list.SetPipelineSRV(9, 1, ctx.Get(resources.Clusters.Counts));
				list.SetPipelineSRV(10, 1, ctx.Get(resources.Clusters.Offsets));
				list.SetPipelineSRV(11, 1, ctx.Get(resources.Clusters.Lights));
				list.SetPipelineCBV(0, 1, ctx.ViewCB);
				list.SetPipelineSRV(0, 2, RenderMaterial.MaterialBuffer);
			}

			var geometry = instance.Mesh.RenderData;

			list.SetPipelineConstants(0, 0, instance.InstanceID);
			list.DrawIndexed((int)geometry.IndexHandle.Size, (int)geometry.IndexHandle.Offset);
		}

		list.EndEvent();
	}
}

class TransparentShaderPermutation : ShaderPermutation
{
	private static readonly ShaderModule vertexModule = new(Embed.GetString("Shaders/Standard/Forward/ForwardVS.hlsl"), ShaderStage.Vertex);
	private static readonly ShaderModule pixelModule = new(Embed.GetString("Shaders/Standard/Forward/TransparentPS.hlsl"));

	/// <summary>Null for opaque stacks, which never reach the forward pass.</summary>
	public PipelineState? PSO { get; private set; }

	public override void Init(ShaderModule module)
	{
		BlendMode blendMode = Shaders.Max(o => o.BlendMode);
		if (blendMode is BlendMode.Opaque or BlendMode.Masked)
		{
			return;
		}

		PSO = new PipelineState()
			.SetVertexShader(vertexModule)
			.SetPixelShader(pixelModule.Link("main", ShaderStage.Pixel, module))
			.AsRootConstant(0, 1)
			.SetDepthMode(DepthMode.GreaterEqual, true, false)
			.SetCullMode(Shaders.Max(o => o.FaceMode) == FaceMode.TwoSided ? CullMode.None : CullMode.CCW)
			.SetBlendMode(blendMode == BlendMode.Additive ? BlendPreset.Additive : BlendPreset.AlphaOver)
			.SetRTFormat(0, Vortice.DXGI.Format.R16G16B16A16_Float)
			.Compile().Result;
	}

	public override void Dispose()
	{
		PSO?.Dispose();
	}
}
