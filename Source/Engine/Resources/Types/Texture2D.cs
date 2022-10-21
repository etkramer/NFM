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
		public bool IsLinear { get; } = true;

		public Texture2D(int width, int height, bool isLinear = true, byte mipCount = 4)
		{
			Width = width;
			Height = height;

			IsLinear = isLinear;
			Resource = new Texture(Width, Height, mipCount, isLinear ? Format.R8G8B8A8_UNorm : Format.R8G8B8A8_UNorm_SRgb);
		}

		/// <summary>
		/// Loads raw image data into the texture
		/// </summary>
		public void LoadData(Span<byte> data, int mipLevel = 0)
		{
			// Create resource and upload texture data.
			Renderer.DefaultCommandList.UploadTexture(Resource, data, mipLevel);
		}

		/// <summary>
		/// Automatically generates mipmaps. Compressed textures are not supported.
		/// </summary>
		public void GenerateMips()
		{
			Renderer.DefaultCommandList.GenerateMips(Resource);
		}

		public override void Dispose()
		{
			Resource.Dispose();
		}

		/// <summary>
		/// Creates a new Texture2D from the given color
		/// </summary>
		public static Texture2D FromColor(Color color)
		{
			byte[] data = new byte[1 * 1 * 4];
			for (int i = 0; i < data.Length / 4; i += 4)
			{
				data[i] = (byte)(color.R * 255);
				data[i + 1] = (byte)(color.G * 255);
				data[i + 2] = (byte)(color.B * 255);
				data[i + 3] = (byte)(color.A * 255);
			};

			Texture2D texture = new Texture2D(1, 1, true, 1);
			texture.LoadData(data);
			return texture;
		}

		public static Texture2D White = FromColor(Color.White);
		public static Texture2D Black = FromColor(Color.Black);
		public static Texture2D Normal = FromColor(new Color(0.5f, 0.5f, 1));
		public static Texture2D Purple = FromColor(new Color(1, 0, 1));
	}
}