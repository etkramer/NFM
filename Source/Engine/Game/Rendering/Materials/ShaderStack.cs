using System;
using Engine.Content;
using Engine.GPU;
using Engine.Resources;
using Engine.World;

namespace Engine.Rendering
{
	public class ShaderStack
	{
		public ShaderParameter[] Parameters;
		public ObservableCollection<Shader> Shaders = new();

		public ShaderStack(Shader baseShader)
		{
			Shaders.Add(baseShader);
			Parameters = Shaders.SelectMany(o => o.Parameters).ToArray();

			// Each material instance has one of these - only PSOs need to be cached
		}
	}
}