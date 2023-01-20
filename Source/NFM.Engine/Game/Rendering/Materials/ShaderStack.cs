using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using NFM.GPU;
using NFM.Resources;
using NFM.World;

namespace NFM.Rendering;

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
			CurrentPermutation = new ShaderPermutation(Shaders.ToArray());
		}
	}
}