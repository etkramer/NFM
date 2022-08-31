﻿using System;

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
		public Type Type;
	}

	public class Shader : Resource
	{
		public string ShaderSource { get; set; }

		public List<ShaderParameter> Parameters { get; } = new();
		public BlendMode BlendMode = BlendMode.Opaque;

		public Shader(string source)
		{
			ShaderSource = source;
		}

		public Shader AddColor(string param, Color defaultValue = default)
		{
			Parameters.Add(new ShaderParameter()
			{
				Name = param,
				Value = defaultValue,
				Type = typeof(Color)
			});

			return this;
		}

		public Shader AddTexture(string param, Texture2D defaultValue = default)
		{
			Parameters.Add(new ShaderParameter()
			{
				Name = param,
				Value = defaultValue,
				Type = typeof(Texture2D)
			});

			return this;
		}

		public void SetBlendMode(BlendMode mode)
		{
			BlendMode = mode;
		}
	}
}