using System;

namespace Engine.Resources
{
	public sealed class Material : Resource
	{
		public Material(Shader shader)
		{

		}

		public Material SetBool(string param, bool value)
		{
			return this;
		}

		public Material SetInt(string param, int value)
		{
			return this;
		}

		public Material SetFloat(string param, float value)
		{
			return this;
		}

		public Material SetTexture(string param, Texture2D value)
		{
			return this;
		}
	}
}