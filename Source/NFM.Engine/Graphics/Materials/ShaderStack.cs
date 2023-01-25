using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using NFM.GPU;
using NFM.Resources;
using NFM.World;

namespace NFM.Graphics;

public class ShaderStack
{
	public ShaderParameter[] Parameters;
	public ObservableCollection<Shader> Shaders = new();

	public ShaderStack(Shader baseShader)
	{
		Shaders.Add(baseShader);
		Parameters = Shaders.SelectMany(o => o.Parameters).ToArray();
	}
}