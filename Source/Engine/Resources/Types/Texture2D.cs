using System;
using Engine.GPU;
using Engine.Rendering;
using Vortice.DXGI;

namespace Engine.Resources
{
	public enum TextureCompression
	{
		None = 0,

		// Block Compression
		BC1,
		BC2,
		BC3,
		BC5,

		// DXT aliases
		DXT1 = BC1,
		DXT3 = BC2,
		DXT5 = BC3,
	}

	public sealed class Texture2D : Resource
	{
		internal Texture Resource = null;

		public int Width { get; }
		public int Height { get; }
		public bool IsLinear { get; set; }

		public Texture2D(int width, int height)
		{
			Width = width;
			Height = height;
		}

		/// <summary>
		/// Loads raw image data into the texture.
		/// </summary>
		public void LoadData(Span<byte> data, TextureCompression sourceCompression = TextureCompression.None)
		{
			/*Format format = IsLinear ? Format.R8G8B8A8_UNorm : Format.R8G8B8A8_UNorm_SRgb;

			Resource = new Texture(Width, Height, 1, format);
			Renderer.DefaultCommandList.UploadTexture(Resource, data);*/
		}
	}
}