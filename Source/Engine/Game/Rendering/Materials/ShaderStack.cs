using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Engine.GPU;
using Engine.Resources;
using Engine.World;

namespace Engine.Rendering
{
	public class ShaderStack
	{
		public ShaderParameter[] Parameters;
		public ObservableCollection<Shader> Shaders = new();

		public ShaderPermutation CurrentPermutation { get; private set; }

		public ShaderStack(Shader baseShader)
		{
			Shaders.Add(baseShader);
			Parameters = Shaders.SelectMany(o => o.Parameters).ToArray();

			// Check if the needed permutation already exists.
			if (ShaderPermutation.All.TryFirst(o => o.Shaders.SequenceEqual(Shaders), out var permutation))
			{
				CurrentPermutation = permutation;
			}
			// Otherwise, compile a whole one.
			else
			{
				CurrentPermutation = new ShaderPermutation(Shaders.ToArray(), Parameters);
			}
		}
	}
}