using System;
using Engine.Content;
using Engine.GPU;
using Engine.Resources;
using Engine.World;

namespace Engine.Rendering
{
	public class ShaderStack
	{
		public static List<(ShaderStack, (ShaderProgram, CommandSignature))> Programs { get; } = new();

		public ShaderParameter[] Parameters;
		public ObservableCollection<Shader> Shaders = new();

		public ShaderProgram Program { get; private set; }
		public int ProgramID => Programs.FindIndex(o => o.Item2.Item1 == Program);

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
				// Build program from source code.
				string source = Embed.GetString("HLSL/Material/BaseMaterialPS.hlsl");
				source = source.Replace("#insert MATERIAL", Shaders.First().ShaderSource);

				// Compile program.
				ShaderProgram program = new ShaderProgram()
					.UseIncludes(typeof(Embed).Assembly)
					.SetMeshShader(Embed.GetString("HLSL/BaseMS.hlsl"))
					.SetPixelShader(source, "MaterialPS")
					.SetDepthMode(DepthMode.Equal, true, false)
					.SetCullMode(CullMode.CCW)
					.SetRTCount(2)
					.AsRootConstant(0, 1)
					.Compile().Result;

				// Compile matching command signature.
				CommandSignature signature = new CommandSignature()
					.AddConstantArg(0, program)
					.AddDispatchMeshArg()
					.Compile();

				Programs.Add(new(this, new(program, signature)));
				return program;
			}
		}
	}
}