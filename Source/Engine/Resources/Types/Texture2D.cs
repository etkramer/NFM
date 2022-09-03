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
		public bool IsLinear { get; set; } = true;

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
			// Select D3D texture format from compression mode.
			Format format = sourceCompression switch
			{
				TextureCompression.None when IsLinear => Format.R8G8B8A8_UNorm,
				TextureCompression.None => Format.R8G8B8A8_UNorm_SRgb,
				TextureCompression.BC1 when IsLinear => Format.BC1_UNorm,
				TextureCompression.BC1 => Format.BC1_UNorm_SRgb,
				TextureCompression.BC2 when IsLinear => Format.BC2_UNorm,
				TextureCompression.BC2 => Format.BC2_UNorm_SRgb,
				TextureCompression.BC3 when IsLinear => Format.BC3_UNorm,
				TextureCompression.BC3 => Format.BC3_UNorm_SRgb,
				TextureCompression.BC5 => Format.BC5_UNorm,
				_ => Format.Unknown
			};

			// Create resource and upload texture data.
			Resource = new Texture(Width, Height, 1, format);
			Renderer.DefaultCommandList.UploadTexture(Resource, data);
		}

		public override void Dispose()
		{
			Resource.Dispose();
		}

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

			Texture2D texture = new Texture2D(1, 1);
			texture.LoadData(data);
			return texture;
		}

		public static Texture2D White = FromColor(Color.White);
		public static Texture2D Black = FromColor(Color.Black);
		public static Texture2D Normal = FromColor(new Color(0.5f, 0.5f, 1));
		public static Texture2D Purple = FromColor(new Color(1, 0, 1));
	}
}