using System;
using System.Runtime.CompilerServices;
using Engine.Content;
using Engine.GPU;
using Engine.Resources;
using Engine.World;

namespace Engine.Rendering
{
	public class ShaderStack
	{
		public static List<(ShaderStack, (ShaderProgram, CommandSignature))> Programs { get; } = new();
		private static int lastProgramID = 0;

		public ShaderParameter[] Parameters;
		public ObservableCollection<Shader> Shaders = new();

		public ShaderProgram Program { get; private set; }
		public int ProgramID { get; private set; } = 0;

		public ShaderStack(Shader baseShader)
		{
			Shaders.Add(baseShader);
			Parameters = Shaders.SelectMany(o => o.Parameters).ToArray();

			Program = GetProgram();
		}

		private ShaderProgram GetProgram()
		{
			// Check if the needed program already exists.
			if (Programs.TryFirst(o => o.Item1 != this && o.Item1.Shaders.SequenceEqual(Shaders), out var pair))
			{
				ShaderProgram program = pair.Item2.Item1;
				return program;
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
					int paramSize = 0;

					// Override sizes where needed.
					switch (param.Value)
					{
						// Descriptor handles
						case Texture2D:
							paramSize = sizeof(uint);
							break;
						// Enforce minimum 4b alignment
						case bool:
						case byte:
						case sbyte:
							paramSize = Marshal.SizeOf(typeof(int));
							break;
						default:
							paramSize = Marshal.SizeOf(param.Type);
							break;
					}

					// Write actual loader statements.
					switch (param.Value)
					{
						case Texture2D:
							setupSource += $"{param.Name} = ResourceDescriptorHeap[MaterialParams.Load(materialID + {paramOffset})];\n";
							break;
						case bool:
							setupSource += $"{param.Name} = (bool)MaterialParams.Load(materialID + {paramOffset});\n";
							break;
						case sbyte:
						case int:
							setupSource += $"{param.Name} = asint(MaterialParams.Load(materialID + {paramOffset}));\n";
							break;
						case byte:
						case uint:
						case char:
							setupSource += $"{param.Name} = asuint(MaterialParams.Load(materialID + {paramOffset}));\n";
							break;
						case float:
							setupSource += $"{param.Name} = asfloat(MaterialParams.Load(materialID + {paramOffset}));\n";
							break;
						case Color:
						case Vector4:
							setupSource += $"{param.Name} = asfloat(MaterialParams.Load4(materialID + {paramOffset}));\n";
							break;
						case Vector3:
							setupSource += $"{param.Name} = asfloat(MaterialParams.Load3(materialID + {paramOffset}));\n";
							break;
						case Vector2:
							setupSource += $"{param.Name} = asfloat(MaterialParams.Load2(materialID + {paramOffset}));\n";
							break;
						case Vector4i:
							setupSource += $"{param.Name} = asint(MaterialParams.Load4(materialID + {paramOffset});\n";
							break;
						case Vector3i:
							setupSource += $"{param.Name} = asint(MaterialParams.Load3(materialID + {paramOffset}));\n";
							break;
						case Vector2i:
							setupSource += $"{param.Name} = asint(MaterialParams.Load2(materialID + {paramOffset}));\n";
							break;
						default:
							throw new NotSupportedException($"{param.Type.Name} is not a supported shader parameter type");
					}

					paramOffset += paramSize;
				}

				// Build program from source code.
				string source = Embed.GetString("HLSL/Material/BaseMaterialPS.hlsl");
				source = source.Replace("#insert SURFACE", surfaceSource).Replace("#insert SETUP", setupSource);

				// Compile program.
				ShaderProgram program = new ShaderProgram()
					.UseIncludes(typeof(Embed).Assembly)
					.SetMeshShader(Embed.GetString("HLSL/BaseMS.hlsl"))
					.SetPixelShader(source, "MaterialPS")
					.SetDepthMode(DepthMode.Equal, true, false)
					.SetCullMode(CullMode.CCW)
					.AsRootConstant(0, 1)
					.Compile().Result;

				// Compile matching command signature.
				CommandSignature signature = new CommandSignature()
					.AddConstantArg(0, program)
					.AddDispatchMeshArg()
					.Compile();

				ProgramID = lastProgramID++;
				Programs.Add(new(this, new(program, signature)));
				return program;
			}
		}
	}
}