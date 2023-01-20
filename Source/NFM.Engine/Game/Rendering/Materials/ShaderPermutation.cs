using System;
using NFM.GPU;
using NFM.Resources;

namespace NFM.Rendering;

public class ShaderPermutation : IDisposable
{
	public static List<ShaderPermutation> All { get; } = new();
	private static int lastShaderID = 0;

	public int ShaderID { get; private set; } = 0;
	public Shader[] Shaders { get; private set; }

	public PipelineState MaterialPSO { get; private set; }
	public CommandSignature MaterialSignature { get; private set; }

	public ShaderPermutation(Shader[] shaders)
	{
		Shaders = shaders;
		string baseSource = shaders[0].ShaderSource;
		string setupSource = "";

		// Loop over shader parameters and add setup code for each.
		int paramOffset = 4;
		foreach (var param in shaders.SelectMany(o => o.Parameters).Distinct())
		{
			// Override sizes where needed.
			int paramSize = param.Value switch
			{
				Texture2D => sizeof(uint),
				bool or byte or sbyte => Marshal.SizeOf(typeof(int)),
				_ => Marshal.SizeOf(param.Type)
			};

			// Add HLSL code for loading parameters
			setupSource += param.Value switch
			{
				Texture2D => $"{param.Name} = ResourceDescriptorHeap[MaterialParams.Load(materialID + {paramOffset})];\n",
				bool => $"{param.Name} = (bool)MaterialParams.Load(materialID + {paramOffset});\n",
				int or sbyte => $"{param.Name} = asint(MaterialParams.Load(materialID + {paramOffset}));\n",
				uint or byte => $"{param.Name} = asuint(MaterialParams.Load(materialID + {paramOffset}));\n",
				float => $"{param.Name} = asfloat(MaterialParams.Load(materialID + {paramOffset}));\n",
				Vector4 or Color => $"{param.Name} = asfloat(MaterialParams.Load4(materialID + {paramOffset}));\n",
				Vector3 => $"{param.Name} = asfloat(MaterialParams.Load3(materialID + {paramOffset}));\n",
				Vector2 => $"{param.Name} = asfloat(MaterialParams.Load2(materialID + {paramOffset}));\n",
				Vector4i => $"{param.Name} = asint(MaterialParams.Load4(materialID + {paramOffset}));\n",
				Vector3i => $"{param.Name} = asint(MaterialParams.Load3(materialID + {paramOffset}));\n",
				Vector2i => $"{param.Name} = asint(MaterialParams.Load2(materialID + {paramOffset}));\n",

				_ => throw new NotSupportedException($"{param.Type.Name} is not a supported shader parameter type")
			};

			paramOffset += paramSize;
		}

		// Build PSO from source code.
		string surfaceTemplate = Embed.GetString("Shaders/Geometry/Material/BaseMaterialPS.hlsl", typeof(Engine).Assembly);
		string surfaceShaderSource = surfaceTemplate.Replace("#insert SURFACE", baseSource).Replace("#insert SETUP", setupSource);

		// Compile PSO.
		MaterialPSO = new PipelineState()
			.UseIncludes(typeof(Engine).Assembly)
			.SetMeshShader(Embed.GetString("Shaders/Geometry/Shared/BaseMS.hlsl", typeof(Engine).Assembly), "BaseMS")
			.SetPixelShader(surfaceShaderSource, "MaterialPS")
			.SetDepthMode(DepthMode.Equal, true, false)
			.AsRootConstant(0, 1)
			.Compile().Result;

		// Compile matching indirect command signature.
		MaterialSignature = new CommandSignature()
			.AddConstantArg(0, MaterialPSO)
			.AddDispatchMeshArg()
			.Compile();

		ShaderID = lastShaderID++;
		All.Add(this);
	}

	public void Dispose()
	{
		MaterialSignature.Dispose();
		MaterialPSO.Dispose();
	}
}
