using System;

namespace Engine.Resources
{
	public enum BlendMode
	{
		Opaque,
		Transparent
	}

	public class Shader : Resource
	{
		public string ShaderSource { get; set; }

		public Shader(string source)
		{
			ShaderSource = source;
		}

		public void AddParam<T>(string name, T defaultValue = default)
		{
			
		}

		public void SetBlendMode(BlendMode mode)
		{

		}

		public override void OnLoad()
		{
			base.OnLoad();
		}
	}
}