using System;

namespace Engine.Resources
{
	public enum BlendMode
	{
		Opaque,
		Transparent
	}

	public struct ShaderParameter
	{
		public string Name;
		public object Value;
	}

	public class Shader : Resource
	{
		public string ShaderSource { get; set; }

		public int ParameterCount => Parameters.Count;
		public List<ShaderParameter> Parameters { get; } = new();
		public BlendMode BlendMode = BlendMode.Opaque;

		public Shader(string source)
		{
			ShaderSource = source;
		}

		public Shader AddInt(string param, int defaultValue = default)
		{
			Parameters.Add(new ShaderParameter()
			{
				Name = param,
				Value = defaultValue
			});

			return this;
		}

		public void SetBlendMode(BlendMode mode)
		{
			BlendMode = mode;
		}
	}
}