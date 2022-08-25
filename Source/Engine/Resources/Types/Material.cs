using System;

namespace Engine.Resources
{
	public sealed class Material : Resource
	{
		public Shader Shader { get; }

		public Material(Shader shader)
		{
			Shader = shader;
		}

		public Material SetBool(string param, bool value)
		{
			return this;
		}

		public Material SetInt(string param, int value)
		{
			return this;
		}

		public Material SetTexture(string param, Texture2D value)
		{
			return this;
		}
	}
}