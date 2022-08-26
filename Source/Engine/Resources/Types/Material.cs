using System;

namespace Engine.Resources
{
	public sealed class Material : Resource
	{
		public Shader Shader { get; }

		public List<ShaderParameter> OverrideParameters { get; } = new();

		public Material(Shader shader)
		{
			Shader = shader;
		}

		public Material SetInt(string param, int value)
		{
			OverrideParameters.Add(new ShaderParameter()
			{
				Name = param,
				Value = value
			});

			return this;
		}
	}
}