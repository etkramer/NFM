using System;
using System.Runtime.InteropServices;
using System.Reflection;
using SharpGen.Runtime;
using Vortice.Direct3D12;
using Vortice.Dxc;
using Vortice.DXGI;
using Vortice.Direct3D12.Shader;
using Vortice.Direct3D;

namespace NFM.GPU;

public enum CullMode
{
	None = Vortice.Direct3D12.CullMode.None,
	CW = Vortice.Direct3D12.CullMode.Back,
	CCW = Vortice.Direct3D12.CullMode.Front
}

public enum DepthMode
{
	None = ComparisonFunction.None,
	Never = ComparisonFunction.Never,
	Less = ComparisonFunction.Less,
	Equal = ComparisonFunction.Equal,
	LessEqual = ComparisonFunction.LessEqual,
	Greater = ComparisonFunction.Greater,
	NotEqual = ComparisonFunction.NotEqual,
	GreaterEqual = ComparisonFunction.GreaterEqual,
	Always = ComparisonFunction.Always
}

public enum TopologyType
{
	Triangle = PrimitiveTopologyType.Triangle,
	Line = PrimitiveTopologyType.Line
}

public sealed class PipelineState : IDisposable
{
	public bool IsGraphics { get; private set; } = false;
	public bool IsCompute => !IsGraphics;

	static readonly DxcCompilerOptions compilerOptions = new()
	{
		ShaderModel = DxcShaderModel.Model6_6,
		HLSLVersion = 2021,
		SkipOptimizations = Debug.IsDebugMode,
		AllResourcesBound = true,
	};

	// Compiled program
	internal ID3D12PipelineState PSO = null;
	internal ID3D12RootSignature RootSignature = null;

	// RootParameter <-> register mappings. TODO: Doesn't account for register spaces.
	internal Dictionary<BindPoint, int> tRegisterMapping = new();
	internal Dictionary<BindPoint, int> uRegisterMapping = new();
	internal Dictionary<BindPoint, int> cRegisterMapping = new();

	// Parameters
	private List<RootParameter1> rootParams = new();
	private ShaderModule compiledCompute;
	private ShaderModule compiledVertex;
	private ShaderModule compiledMesh;
	private ShaderModule compiledPixel;
	private CullMode cullMode = CullMode.None;
	private DepthMode depthMode = DepthMode.None;
	private bool depthRead = false;
	private bool depthWrite = false;
	private Format[] rtFormats = { D3DContext.RTFormat };
	private int rtSamples = 1;
	private TopologyType topologyType = TopologyType.Triangle;
	private bool isBlendEnabled = false;

	public void Dispose()
	{
		PSO.Dispose();
		RootSignature.Dispose();
	}

	public PipelineState SetRTSamples(int samples)
	{
		rtSamples = samples;
		return this;
	}

	public PipelineState SetRTFormat(int rt, Format format)
	{
		rtFormats[rt] = format;
		return this;
	}

	public PipelineState SetRTCount(int count)
	{
		rtFormats = new Format[count];
		for (int i = 0; i < count; i++)
		{
			rtFormats[i] = Format.R8G8B8A8_UNorm;
		}

		return this;
	}

	public PipelineState SetCullMode(CullMode mode)
	{
		cullMode = mode;
		return this;
	}

	public PipelineState SetDepthMode(DepthMode mode, bool read, bool write)
	{
		depthMode = mode;
		depthRead = read;
		depthWrite = write;
		return this;
	}

	public PipelineState SetTopologyType(TopologyType type)
	{
		topologyType = type;
		return this;
	}

	public PipelineState SetEnableBlend(bool value)
	{
		isBlendEnabled = value;
		return this;
	}

	public PipelineState SetComputeShader(ShaderModule module)
	{
		compiledCompute = module;
		IsGraphics = false;
		return this;
	}

	public PipelineState SetVertexShader(ShaderModule module)
	{
		compiledVertex = module;
		IsGraphics = true;
		return this;
	}

	public PipelineState SetMeshShader(ShaderModule module)
	{
		compiledMesh = module;
		IsGraphics = true;
		return this;
	}

	public PipelineState SetPixelShader(ShaderModule module)
	{
		compiledPixel = module;
		IsGraphics = true;
		return this;
	}

	/// <summary>
	/// Specifies that a parameter should be interpreted as a 32-bit root constant.
	/// </summary>
	public PipelineState AsRootConstant(int slot, int count, int space = 0)
	{
		rootParams.Add(new RootParameter1(new RootConstants(slot, space, count), ShaderVisibility.All));
		cRegisterMapping.Add(new(slot, space), rootParams.Count - 1);
		return this;
	}

	private RootParameter1[] BuildRootParameters(params ShaderModule[] shaders)
	{
		// Loop through all participating shaders.
		foreach (ShaderModule shader in shaders)
		{
			if (shader != null)
			{
				// Build reflection data.
				if (!DxcCompiler.Utils.CreateReflection(CreateBlob(shader.Bytecode.ToArray()), out ID3D12ShaderReflection reflection).Success)
				{
					// Failed to build reflection data - shader is probably invalid.
					return new RootParameter1[0];
				}

				for (int i = 0; i < reflection.Description.BoundResources; i++)
				{
					var binding = reflection.GetResourceBindingDescription(i);

					// Check if resource is UAV.
					bool UAV = binding.Type == ShaderInputType.UnorderedAccessViewAppendStructured;
					UAV |= binding.Type == ShaderInputType.UnorderedAccessViewConsumeStructured;
					UAV |= binding.Type == ShaderInputType.UnorderedAccessViewFeedbacktexture;
					UAV |= binding.Type == ShaderInputType.UnorderedAccessViewRWByteAddress;
					UAV |= binding.Type == ShaderInputType.UnorderedAccessViewRWStructured;
					UAV |= binding.Type == ShaderInputType.UnorderedAccessViewRWStructuredWithCounter;
					UAV |= binding.Type == ShaderInputType.UnorderedAccessViewRWTyped;
					bool CBV = binding.Type == ShaderInputType.ConstantBuffer;
					bool SRV = !UAV && !CBV;

					if (SRV)
					{
						if (!tRegisterMapping.ContainsKey(new(binding.BindPoint, binding.Space)))
						{
							DescriptorRange1 range = new DescriptorRange1(DescriptorRangeType.ShaderResourceView, 1, binding.BindPoint, binding.Space);
							rootParams.Add(new RootParameter1(new RootDescriptorTable1(range), ShaderVisibility.All));
						
							tRegisterMapping.Add(new(binding.BindPoint, binding.Space), rootParams.Count - 1);
						}
					}
					else if (UAV)
					{
						if (!uRegisterMapping.ContainsKey(new(binding.BindPoint, binding.Space)))
						{
							DescriptorRange1 range = new DescriptorRange1(DescriptorRangeType.UnorderedAccessView, 1, binding.BindPoint, binding.Space);
							rootParams.Add(new RootParameter1(new RootDescriptorTable1(range), ShaderVisibility.All));

							uRegisterMapping.Add(new(binding.BindPoint, binding.Space), rootParams.Count - 1);
						}
					}
					else if (CBV)
					{
						if (!cRegisterMapping.ContainsKey(new(binding.BindPoint, binding.Space)))
						{
							DescriptorRange1 range = new DescriptorRange1(DescriptorRangeType.ConstantBufferView, 1, binding.BindPoint, binding.Space);
							rootParams.Add(new RootParameter1(new RootDescriptorTable1(range), ShaderVisibility.All));

							cRegisterMapping.Add(new(binding.BindPoint, binding.Space), rootParams.Count - 1);
						}
					}
				}
			}
		}

		return rootParams.ToArray();
	}

	private unsafe Result CreatePipelineState<T, TData>(TData data, out T result) where T : ID3D12PipelineState where TData : unmanaged
	{
		PipelineStateStreamDescription description = new()
		{
			SizeInBytes = sizeof(TData),
			SubObjectStream = new IntPtr(&data)
		};

		return D3DContext.Device.CreatePipelineState(description, out result);
	}

	/// <summary>
	/// Compile and validate the PSO.
	/// </summary>
	public Task<PipelineState> Compile()
	{
		return Task.Run(() =>
		{
			if (IsGraphics && (compiledPixel == null || (compiledVertex == null && compiledMesh == null)))
			{
				throw new NotSupportedException("Cannot use a pixel shader without a vertex/mesh shader, or vice versa");
			}

			if (IsCompute && IsGraphics)
			{
				throw new NotSupportedException("Cannot use a compute shader in the same PSO as a vertex/mesh or pixel shader");
			}

			// Build root parameters.
			RootParameter1[] rootParameters = BuildRootParameters(compiledVertex, compiledMesh, compiledPixel, compiledCompute);

			// Create static samplers.
			StaticSamplerDescription[] staticSamplers = new[]
			{
				new StaticSamplerDescription(SamplerDescription.AnisotropicWrap, ShaderVisibility.All, 0, 1) { MaxAnisotropy = 16 },
				new StaticSamplerDescription(SamplerDescription.LinearWrap, ShaderVisibility.All, 1, 1),
				new StaticSamplerDescription(SamplerDescription.PointWrap, ShaderVisibility.All, 2, 1)
			};

			// Create root signature.
			RootSignature = D3DContext.Device.CreateRootSignature(new RootSignatureDescription1(
				RootSignatureFlags.ConstantBufferViewShaderResourceViewUnorderedAccessViewHeapDirectlyIndexed, rootParameters, staticSamplers));

			bool useDepth = depthWrite || (depthRead && depthMode != DepthMode.None);

			// Create PSO.
			Result result = default;
			if (IsGraphics)
			{
				BlendDescription disabledBlend = BlendDescription.Opaque;
				for (int i = 0; i < D3D12.SimultaneousRenderTargetCount; i++)
				{
					disabledBlend.RenderTarget[i].BlendEnable = false;
				}

				if (compiledVertex == null)
				{
					result = CreatePipelineState(new MeshPipelineStateStream()
					{
						RootSignature = RootSignature,
						MeshShader = new ShaderBytecode(compiledMesh.Bytecode.ToArray()),
						PixelShader = new ShaderBytecode(compiledPixel.Bytecode.ToArray()),
						SampleMask = uint.MaxValue,
						PrimitiveTopology = (PrimitiveTopologyType)topologyType,
						SampleDescription = new SampleDescription(rtSamples, rtSamples == 1 ? 0 : 1),
						RenderTargetFormats = rtFormats,
						DepthStencilFormat = D3DContext.DSFormat,
						DepthStencilState = useDepth ? new DepthStencilDescription(true, depthWrite ? DepthWriteMask.All : DepthWriteMask.Zero, (ComparisonFunction)depthMode) : DepthStencilDescription.None,
						RasterizerState = new RasterizerDescription()
						{
							CullMode = (Vortice.Direct3D12.CullMode)cullMode,
							FillMode = FillMode.Solid,
							AntialiasedLineEnable = topologyType == TopologyType.Line,
						},
						BlendDescription = isBlendEnabled ? BlendDescription.AlphaBlend : disabledBlend
					}, out PSO);
				}
				else
				{
					result = CreatePipelineState(new VertexPipelineStateStream()
					{
						RootSignature = RootSignature,
						VertexShader = new ShaderBytecode(compiledVertex.Bytecode.ToArray()),
						PixelShader = new ShaderBytecode(compiledPixel.Bytecode.ToArray()),
						SampleMask = uint.MaxValue,
						PrimitiveTopology = (PrimitiveTopologyType)topologyType,
						SampleDescription = new SampleDescription(rtSamples, rtSamples == 1 ? 0 : 1),
						RenderTargetFormats = rtFormats,
						DepthStencilFormat = D3DContext.DSFormat,
						DepthStencilState = useDepth ? new DepthStencilDescription(true, depthWrite ? DepthWriteMask.All : DepthWriteMask.Zero, (ComparisonFunction)depthMode) : DepthStencilDescription.None,
						RasterizerState = new RasterizerDescription()
						{
							CullMode = (Vortice.Direct3D12.CullMode)cullMode,
							FillMode = FillMode.Solid,
							AntialiasedLineEnable = topologyType == TopologyType.Line,
						},
						BlendDescription = isBlendEnabled ? BlendDescription.AlphaBlend : disabledBlend
					}, out PSO);
				}
			}
			else if (IsCompute)
			{
				result = CreatePipelineState(new ComputePipelineStateStream()
				{
					RootSignature = RootSignature,
					ComputeShader = new ShaderBytecode(compiledCompute.Bytecode.ToArray()),
				}, out PSO);
			}

			// Check for errors.
			if (!result.Success)
			{
				throw new InvalidProgramException($"PSO failed to compile with message \"{result.Description.Trim()}\" ({result.NativeApiCode})");
			}

			return this;
		});
	}

	internal static IDxcBlob CreateBlob(byte[] data)
	{
		GCHandle dataPointer = GCHandle.Alloc(data, GCHandleType.Pinned);
		DxcCompiler.Utils.CreateBlob(dataPointer.AddrOfPinnedObject(), data.Length, Dxc.DXC_CP_UTF8, out IDxcBlobEncoding blob).CheckError();
		dataPointer.Free();
			
		return blob;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct CommonPipelineStateStream
	{
		public PipelineStateSubObjectTypeRootSignature RootSignature;
		public PipelineStateSubObjectTypeVertexShader VertexShader;
		public PipelineStateSubObjectTypeMeshShader MeshShader;
		public PipelineStateSubObjectTypePixelShader PixelShader;
		public PipelineStateSubObjectTypeSampleMask SampleMask;
		public PipelineStateSubObjectTypePrimitiveTopology PrimitiveTopology;
		public PipelineStateSubObjectTypeRasterizer RasterizerState;
		public PipelineStateSubObjectTypeDepthStencil DepthStencilState;
		public PipelineStateSubObjectTypeRenderTargetFormats RenderTargetFormats;
		public PipelineStateSubObjectTypeDepthStencilFormat DepthStencilFormat;
		public PipelineStateSubObjectTypeSampleDescription SampleDescription;
		public PipelineStateSubObjectTypeBlend BlendDescription;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct VertexPipelineStateStream
	{
		public PipelineStateSubObjectTypeRootSignature RootSignature;
		public PipelineStateSubObjectTypeVertexShader VertexShader;
		public PipelineStateSubObjectTypePixelShader PixelShader;
		public PipelineStateSubObjectTypeSampleMask SampleMask;
		public PipelineStateSubObjectTypePrimitiveTopology PrimitiveTopology;
		public PipelineStateSubObjectTypeRasterizer RasterizerState;
		public PipelineStateSubObjectTypeDepthStencil DepthStencilState;
		public PipelineStateSubObjectTypeRenderTargetFormats RenderTargetFormats;
		public PipelineStateSubObjectTypeDepthStencilFormat DepthStencilFormat;
		public PipelineStateSubObjectTypeSampleDescription SampleDescription;
		public PipelineStateSubObjectTypeBlend BlendDescription;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct MeshPipelineStateStream
	{
		public PipelineStateSubObjectTypeRootSignature RootSignature;
		public PipelineStateSubObjectTypeMeshShader MeshShader;
		public PipelineStateSubObjectTypePixelShader PixelShader;
		public PipelineStateSubObjectTypeSampleMask SampleMask;
		public PipelineStateSubObjectTypePrimitiveTopology PrimitiveTopology;
		public PipelineStateSubObjectTypeRasterizer RasterizerState;
		public PipelineStateSubObjectTypeDepthStencil DepthStencilState;
		public PipelineStateSubObjectTypeRenderTargetFormats RenderTargetFormats;
		public PipelineStateSubObjectTypeDepthStencilFormat DepthStencilFormat;
		public PipelineStateSubObjectTypeSampleDescription SampleDescription;
		public PipelineStateSubObjectTypeBlend BlendDescription;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct ComputePipelineStateStream
	{
		public PipelineStateSubObjectTypeRootSignature RootSignature;
		public PipelineStateSubObjectTypeComputeShader ComputeShader;
	}
}

public struct BindPoint
{
	public int Slot;
	public int Space;

	public BindPoint(int slot, int space)
	{
		Slot = slot;
		Space = space;
	}

	public static implicit operator BindPoint(int slot) => new(slot, 0);

	public override string ToString()
	{
		return $"{Slot}, space{Space}";
	}
}