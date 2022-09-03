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

		public void SetColor(string param, Color value)
		{
			OverrideParameters.Add(new ShaderParameter()
			{
				Name = param,
				Value = value
			});
		}

		public void SetTexture(string param, Texture2D value)
		{
			if (value == null)
			{
				return;
			}

			OverrideParameters.Add(new ShaderParameter()
			{
				Name = param,
				Value = value
			});
		}
	}
}