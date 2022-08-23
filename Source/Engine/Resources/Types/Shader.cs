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

		public Shader AddBool(string param, bool defaultValue = default)
		{
			return this;
		}

		public Shader AddInt(string param, bool defaultValue = default)
		{
			return this;
		}

		public Shader AddTexture(string param, Texture2D defaultValue = default)
		{
			return this;
		}

		public void SetBlendMode(BlendMode mode)
		{

		}
	}
}