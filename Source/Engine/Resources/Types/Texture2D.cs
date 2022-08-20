using System;
using Engine.GPU;
using StbImageSharp;
using Vortice.DXGI;

namespace Engine.Resources
{
	public enum TextureFormat
	{
		RGB,
		RGBA,
	}

	public sealed class Texture2D : Resource
	{
		public Texture2D(long width, long height, TextureFormat format = TextureFormat.RGB)
		{

		}

		public Texture2D SetLinear(bool value)
		{
			return this;
		}

		public Texture2D SetCompressed(bool value)
		{
			return this;
		}

		/// <summary>
		/// Loads raw, uncompressed data (in 8-bit SDR format) into the texture.
		/// </summary>
		public Texture2D LoadData(byte[] data)
		{
			return this;
		}

		/// <summary>
		/// Loads raw, uncompressed data (in 32-bit HDR format) into the texture.
		/// </summary>
		public Texture2D LoadData(float[] data)
		{
			return this;
		}

		/// <summary>
		/// Loads compressed data in JPG/PNG/BMP/TGA/PSD/GIF formats into the texture.
		/// </summary>
		public Texture2D LoadData(byte[] data, TextureFormat sourceFormat, bool hdr = false)
		{
			if (hdr)
			{
				ImageResultFloat image = ImageResultFloat.FromMemory(data, sourceFormat == TextureFormat.RGB ? ColorComponents.RedGreenBlue : ColorComponents.RedGreenBlueAlpha);
				return LoadData(image.Data);
			}
			else
			{
				ImageResult image = ImageResult.FromMemory(data, sourceFormat == TextureFormat.RGB ? ColorComponents.RedGreenBlue : ColorComponents.RedGreenBlueAlpha);
				return LoadData(image.Data);
			}
		}
	}
}