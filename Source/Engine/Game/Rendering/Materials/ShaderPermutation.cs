using System;
using Engine.GPU;
using Engine.Resources;

namespace Engine.Rendering
{
	public class ShaderPermutation : IDisposable
	{
		public static List<ShaderPermutation> All { get; } = new();
		private static int lastProgramID = 0;

		public int ShaderID { get; private set; } = 0;

		public Shader[] Shaders { get; private set; }
		public ShaderProgram Program { get; private set; }
		public CommandSignature Signature { get; private set; }

		public ShaderPermutation(Shader[] shaders, ShaderParameter[] parameters)
		{
			Shaders = shaders;
			string baseSource = shaders[0].ShaderSource;
			string setupSource = "";

			// Write setup code for each parameter.
			int paramOffset = 4;
			foreach (var param in parameters)
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

			// Build program from source code.
			string surfaceTemplate = Embed.GetString("Content/Shaders/Geometry/Material/BaseMaterialPS.hlsl", typeof(Game).Assembly);
			string programSource = surfaceTemplate.Replace("#insert SURFACE", baseSource).Replace("#insert SETUP", setupSource);

			// Compile program.
			Program = new ShaderProgram()
				.UseIncludes(typeof(Game).Assembly)
				.SetMeshShader(Embed.GetString("Content/Shaders/Geometry/Shared/BaseMS.hlsl", typeof(Game).Assembly))
				.SetPixelShader(programSource, "MaterialPS")
				.SetDepthMode(DepthMode.Equal, true, false)
				.SetCullMode(CullMode.CCW)
				.AsRootConstant(0, 1)
				.Compile().Result;

			// Compile matching command signature.
			Signature = new CommandSignature()
				.AddConstantArg(0, Program)
				.AddDispatchMeshArg()
				.Compile();

			ShaderID = lastProgramID++;
			All.Add(this);
		}

		public void Dispose()
		{
			Signature.Dispose();
			Program.Dispose();
		}
	}
}
