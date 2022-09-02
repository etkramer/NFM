using System;
using Engine.GPU;
using Engine.Rendering;
using StbiSharp;
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

		private void Commit(Span<byte> data)
		{
			Format format = IsLinear ? Format.R8G8B8A8_UNorm : Format.R8G8B8A8_UNorm_SRgb;
			Resource = new Texture(Width, Height, 1, format);

			Renderer.DefaultCommandList.UploadTexture(Resource, data);
		}

		/// <summary>
		/// Loads raw, uncompressed data (in 8bpc SDR format) into the texture.
		/// </summary>
		public void LoadData(Span<byte> data)
		{
			Commit(data);
		}

		/// <summary>
		/// Loads compressed data in JPG/PNG/BMP/TGA/PSD/GIF formats into the texture.
		/// </summary>
		public void LoadData(byte[] data, TextureFormat sourceFormat)
		{
			// Decompress image
			StbiImage image = Stbi.LoadFromMemory(data, 4);
			Width = image.Width;
			Height = image.Height;

			// ..and load it.
			LoadData(SpanFromReadonly(image.Data));
		}

		private unsafe Span<T> SpanFromReadonly<T>(ReadOnlySpan<T> readOnly) where T : unmanaged
		{
			fixed (T* dataPtr = readOnly)
			{
				return new Span<T>(dataPtr, readOnly.Length * sizeof(T));
			}
		}
	}
}