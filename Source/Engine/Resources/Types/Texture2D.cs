using System;
using Engine.GPU;
using Vortice.DXGI;

namespace Engine.Resources
{
	public enum TextureFormat
	{
		RGB8,
		RGBA8
	}

	public sealed class Texture2D : Resource
	{
		public Texture2D(long width, long height, TextureFormat format = TextureFormat.RGB8)
		{

		}

		public Texture2D SetLinear(bool value)
		{
			return this;
		}

		/// <summary>
		/// Loads raw, uncompressed data into the texture.
		/// </summary>
		public Texture2D LoadData(byte[] data)
		{
			return this;
		}
	}
}