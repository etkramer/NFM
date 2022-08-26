﻿using System;
using System.Runtime.InteropServices;
using System.Reflection;
using SharpGen.Runtime;
using Vortice.Direct3D12;
using Vortice.Dxc;
using Vortice.DXGI;
using Engine.Aspects;
using System.Text;
using Vortice.Direct3D12.Shader;
using Vortice.Direct3D;

namespace Engine.GPU
{
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

	[AutoDispose]
	public sealed class ShaderProgram : IDisposable
	{
		public bool IsCompute { get; private set; } = false;
		public bool IsMeshPixel { get; private set; } = false;

		static readonly DxcCompilerOptions compilerOptions = new()
		{
			ShaderModel = DxcShaderModel.Model6_6,
			HLSLVersion = 2021,
			SkipOptimizations = Debug.IsDebugMode
		};

		// Compiled program
		internal ID3D12PipelineState PSO = null;
		internal ID3D12RootSignature RootSignature = null;

		// RootParameter <-> register mappings. TODO: Doesn't account for register spaces.
		internal Dictionary<Register, int> tRegisterMapping = new();
		internal Dictionary<Register, int> uRegisterMapping = new();
		internal Dictionary<Register, int> cRegisterMapping = new();

		// Parameters
		private List<RootParameter1> rootParams = new();
		private ShaderBytecode compiledCompute;
		private ShaderBytecode compiledMesh;
		private ShaderBytecode compiledPixel;
		private CullMode cullMode = CullMode.None;
		private DepthMode depthMode = DepthMode.None;
		private bool depthRead = false;
		private bool depthWrite = false;
		private Format[] rtFormats = { GPUContext.RTFormat };
		private int rtSamples = 1;

		// Custom handler for #including files from arbitrary file systems
		private CustomIncludeHandler shaderIncludeHandler = null;

		public ShaderProgram()
		{

		}

		public void Dispose()
		{
			PSO.Dispose();
			RootSignature.Dispose();
		}

		public ShaderProgram SetRTSamples(int samples)
		{
			rtSamples = samples;
			return this;
		}

		public ShaderProgram SetRTFormat(int rt, Format format)
		{
			rtFormats[rt] = format;
			return this;
		}

		public ShaderProgram SetRTCount(int count)
		{
			rtFormats = new Format[count];
			for (int i = 0; i < count; i++)
			{
				rtFormats[i] = Format.R8G8B8A8_UNorm;
			}

			return this;
		}

		public ShaderProgram SetCullMode(CullMode mode)
		{
			cullMode = mode;
			return this;
		}

		public ShaderProgram SetDepthMode(DepthMode mode, bool read, bool write)
		{
			depthMode = mode;
			depthRead = read;
			depthWrite = write;
			return this;
		}

		public ShaderProgram SetComputeShader(string source, string entryPoint = "ComputeEntry")
		{
			IDxcResult computeResult = DxcCompiler.Compile(DxcShaderStage.Compute, source, entryPoint, compilerOptions, includeHandler: shaderIncludeHandler);

			// Check vertex shader for errors.
			if (computeResult.GetStatus().Success)
			{
				compiledCompute = computeResult.GetObjectBytecodeArray();
			}
			else
			{
				Debug.LogError($"Compute shader failed to compile with message: \n{computeResult.GetErrors()}");
			}

			IsCompute = true;
			return this;
		}

		public ShaderProgram SetMeshShader(string source, string entryPoint = "MeshEntry")
		{
			IDxcResult meshResult = DxcCompiler.Compile(DxcShaderStage.Mesh, source, entryPoint, compilerOptions, includeHandler: shaderIncludeHandler);

			// Check vertex shader for errors.
			if (meshResult.GetStatus().Success)
			{
				compiledMesh = meshResult.GetObjectBytecodeArray();
			}
			else
			{
				Debug.LogError($"Mesh shader failed to compile with message: \n{meshResult.GetErrors()}");
			}

			IsMeshPixel = true;
			return this;
		}

		public ShaderProgram SetPixelShader(string source, string entryPoint = "PixelEntry")
		{
			IDxcResult pixelResult = DxcCompiler.Compile(DxcShaderStage.Pixel, source, entryPoint, compilerOptions, includeHandler: shaderIncludeHandler);
			compiledPixel = pixelResult.GetObjectBytecodeArray();

			// Check pixel shader for errors.
			if (pixelResult.GetStatus().Success)
			{
				compiledPixel = pixelResult.GetObjectBytecodeArray();
			}
			else
			{
				Debug.LogError($"Pixel shader failed to compile with message: \n{pixelResult.GetErrors()}");
			}

			IsMeshPixel = true;
			return this;
		}

		/// <summary>
		/// Specifies that a parameter should be interpreted as a 32-bit root constant.
		/// </summary>
		public ShaderProgram Constant<T>(int slot, int space = 0) where T : unmanaged
		{
			unsafe
			{
				int count = sizeof(T) / 32;
				return AsRootConstant(slot, count, space);
			}
		}

		/// <summary>
		/// Specifies that a parameter should be interpreted as a 32-bit root constant.
		/// </summary>
		public ShaderProgram AsRootConstant(int slot, int count, int space = 0)
		{
			rootParams.Add(new RootParameter1(new RootConstants(slot, space, count), ShaderVisibility.All));
			cRegisterMapping.Add(new(slot, space), rootParams.Count - 1);
			return this;
		}

		private RootParameter1[] BuildRootParameters(params ShaderBytecode[] shaders)
		{
			// Loop through all participating shaders.
			foreach (ShaderBytecode shader in shaders)
			{
				if (shader != null)
				{
					// Build reflection data.
					if (!DxcCompiler.Utils.CreateReflection(CreateBlob(shader.Data), out ID3D12ShaderReflection reflection).Success)
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

		[StructLayout(LayoutKind.Sequential)]
		public struct GraphicsPipelineStateStream
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
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct ComputePipelineStateStream
		{
			public PipelineStateSubObjectTypeRootSignature RootSignature;
			public PipelineStateSubObjectTypeComputeShader ComputeShader;
		}

		private unsafe Result CreatePipelineState<T, TData>(TData data, out T result) where T : ID3D12PipelineState where TData : unmanaged
		{
			PipelineStateStreamDescription description = new()
			{
				SizeInBytes = sizeof(TData),
				SubObjectStream = new IntPtr(&data)
			};

			return GPUContext.Device.CreatePipelineState(description, out result);
		}

		/// <summary>
		/// Compile and validate the shader program.
		/// </summary>
		public Task<ShaderProgram> Compile()
		{
			return Task.Run(() =>
			{
				if (IsMeshPixel && (compiledPixel == null || compiledMesh == null))
				{
					throw new NotSupportedException("Cannot use a pixel shader without a mesh shader, or vice versa");
				}

				if (IsCompute && IsMeshPixel)
				{
					throw new NotSupportedException("Cannot use a compute shader in the same program as a mesh or pixel shader");
				}

				// Build root parameters.
				RootParameter1[] rootParameters = BuildRootParameters(compiledMesh, compiledPixel, compiledCompute);

				// Create root signature.
				RootSignature = GPUContext.Device.CreateRootSignature(new RootSignatureDescription1(
					RootSignatureFlags.ConstantBufferViewShaderResourceViewUnorderedAccessViewHeapDirectlyIndexed
					| RootSignatureFlags.SamplerHeapDirectlyIndexed,
					rootParameters)
				);

				bool useDepth = depthWrite || (depthRead && depthMode != DepthMode.None);

				// Create PSO.
				Result result = default;
				if (IsMeshPixel)
				{
					result = CreatePipelineState(new GraphicsPipelineStateStream()
					{
						RootSignature = RootSignature,
						MeshShader = compiledMesh,
						PixelShader = compiledPixel,
						SampleMask = uint.MaxValue,
						PrimitiveTopology = PrimitiveTopologyType.Triangle,
						SampleDescription = new SampleDescription(rtSamples, rtSamples == 1 ? 0 : 1),
						RenderTargetFormats = rtFormats,
						DepthStencilFormat = GPUContext.DSFormat,
						DepthStencilState = useDepth ? new DepthStencilDescription(true, depthWrite ? DepthWriteMask.All : DepthWriteMask.Zero, (ComparisonFunction)depthMode) : DepthStencilDescription.None,
						RasterizerState = new RasterizerDescription((Vortice.Direct3D12.CullMode)cullMode, FillMode.Solid)
					}, out PSO);
				}
				else if (IsCompute)
				{
					result = CreatePipelineState(new ComputePipelineStateStream()
					{
						RootSignature = RootSignature,
						ComputeShader = compiledCompute,
					}, out PSO);
				}

				// Check for errors.
				if (!result.Success)
				{
					throw new InvalidProgramException($"Shader program failed to compile with message \"{result.Description.Trim()}\" ({result.NativeApiCode})");
				}

				return this;
			});
		}

		#region Includes
		sealed class CustomIncludeHandler : CallbackBase, IDxcIncludeHandler
		{
			private Func<string, string> resolveMethod;

			public CustomIncludeHandler(Func<string, string> resolver) => this.resolveMethod = resolver;

			public Result LoadSource(string filename, out IDxcBlob includeSource)
			{
				string source = resolveMethod.Invoke(filename);

				if (source != null)
				{
					includeSource = CreateBlob(Encoding.ASCII.GetBytes(source));
					return Result.Ok;
				}
				else
				{
					includeSource = null;
					return Result.Fail;
				}
			}
		}

		private static IDxcBlob CreateBlob(byte[] data)
		{
			GCHandle dataPointer = GCHandle.Alloc(data, GCHandleType.Pinned);
			DxcCompiler.Utils.CreateBlob(dataPointer.AddrOfPinnedObject(), data.Length, Dxc.DXC_CP_UTF8, out IDxcBlobEncoding blob).CheckError();
			dataPointer.Free();
				
			return blob;
		}

		public ShaderProgram UseIncludes(Assembly embedSource)
		{
			Func<string, string> includeResolver = (path) =>
			{
				path = path.Replace("./", "");
				path = $"{embedSource.GetName().Name}.{path.Replace('/', '.')}";

				using (Stream stream = embedSource.GetManifestResourceStream(path))
				{
					if (stream != null)
					{
						using (StreamReader reader = new(stream))
						{
							return reader.ReadToEnd();
						}
					}
				}

				Debug.LogError($"Failed to resolve include \"{path}\"");
				return null;
			};

			UseIncludes(includeResolver);
			return this;
		}

		public ShaderProgram UseIncludes(Func<string, string> includeResolver)
		{
			shaderIncludeHandler = new CustomIncludeHandler(includeResolver);
			return this;
		}

		#endregion
	}

	public struct Register
	{
		public int Slot;
		public int Space;

		public Register(int slot, int space)
		{
			Slot = slot;
			Space = space;
		}

		public static implicit operator Register(int slot) => new(slot, 0);

		public override string ToString()
		{
			return $"{Slot}, space{Space}";
		}
	}
}
