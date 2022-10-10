using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Avalonia.Controls.Primitives;
using Engine.GPU;
using Engine.Resources;
using Engine.World;

namespace Engine.Rendering
{
	public class ShaderStack
	{
		public static List<ShaderStack> Stacks { get; } = new();
		private static int lastProgramID = 0;

		public ShaderParameter[] Parameters;
		public ObservableCollection<Shader> Shaders = new();

		public ShaderProgram Program { get; private set; }
		public CommandSignature Signature { get; private set; }

		public int ProgramID { get; private set; } = 0;

		public ShaderStack(Shader baseShader)
		{
			Shaders.Add(baseShader);
			Parameters = Shaders.SelectMany(o => o.Parameters).ToArray();

			FindOrCreateProgram();
		}

		private void FindOrCreateProgram()
		{
			// Check if the needed program already exists.
			if (Stacks.TryFirst(o => o.Shaders.SequenceEqual(Shaders), out var stack))
			{
				Program = stack.Program;
				Signature = stack.Signature;
				ProgramID = stack.ProgramID;
			}
			// Otherwise, generate a whole new one.
			else
			{
				string surfaceSource = Shaders.First().ShaderSource;
				string setupSource = "";

				// Write setup code for each parameter.
				int paramOffset = 4;
				foreach (var param in Parameters)
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
				string source = Embed.GetString("Content/Shaders/Material/BaseMaterialPS.hlsl", typeof(Game).Assembly);
				source = source.Replace("#insert SURFACE", surfaceSource).Replace("#insert SETUP", setupSource);

				// Compile program.
				Program = new ShaderProgram()
					.UseIncludes(typeof(Game).Assembly)
					.SetMeshShader(Embed.GetString("Content/Shaders/BaseMS.hlsl", typeof(Game).Assembly))
					.SetPixelShader(source, "MaterialPS")
					.SetDepthMode(DepthMode.Equal, true, false)
					.SetCullMode(CullMode.CCW)
					.AsRootConstant(0, 1)
					.Compile().Result;

				// Compile matching command signature.
				Signature = new CommandSignature()
					.AddConstantArg(0, Program)
					.AddDispatchMeshArg()
					.Compile();

				ProgramID = lastProgramID++;
				Stacks.Add(this);
			}
		}
	}
}