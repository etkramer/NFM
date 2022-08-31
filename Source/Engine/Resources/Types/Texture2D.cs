using System;
using Engine.GPU;
using Engine.Rendering;
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
		internal Texture Resource = null;

		public bool IsLinear { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }

		public Texture2D SetLinear(bool value)
		{
			IsLinear = value;
			return this;
		}

		/// <summary>
		/// Loads raw, uncompressed data (in 8-bit SDR format) into the texture.
		/// </summary>
		public void LoadData(byte[] data)
		{
			Commit(data);
		}

		/// <summary>
		/// Loads raw, uncompressed data (in 32-bit HDR format) into the texture.
		/// </summary>
		public void LoadData(float[] data)
		{
			byte[] byteData = new byte[data.Length * sizeof(float)];
			Buffer.BlockCopy(data, 0, byteData, 0, data.Length * sizeof(float));
			Commit(byteData);
		}

		private void Commit(byte[] data)
		{
			Format format = IsLinear ? Format.R8G8B8A8_UNorm : Format.R8G8B8A8_UNorm_SRgb;
			Resource = new Texture(Width, Height, 1, format);

			Renderer.DefaultCommandList.UploadTexture(Resource, data);
		}

		/// <summary>
		/// Loads compressed data in JPG/PNG/BMP/TGA/PSD/GIF formats into the texture.
		/// </summary>
		public void LoadData(byte[] data, TextureFormat sourceFormat, bool hdr = false)
		{
			if (hdr)
			{
				ImageResultFloat image = ImageResultFloat.FromMemory(data, sourceFormat == TextureFormat.RGB ? ColorComponents.RedGreenBlue : ColorComponents.RedGreenBlueAlpha);
				Width = image.Width;
				Height = image.Height;
				LoadData(image.Data);
			}
			else
			{
				ImageResult image = ImageResult.FromMemory(data, sourceFormat == TextureFormat.RGB ? ColorComponents.RedGreenBlue : ColorComponents.RedGreenBlueAlpha);
				Width = image.Width;
				Height = image.Height;
				LoadData(image.Data);
			}
		}
	}
}