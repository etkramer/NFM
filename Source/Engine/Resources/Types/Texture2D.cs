using System;
using Engine.GPU;
using Engine.Rendering;
using Vortice.DXGI;
using DXGIFormat = Vortice.DXGI.Format;

namespace Engine.Resources
{
	public enum TextureFormat : uint
	{
		RGB,
		RGBA,

		// Floating-point HDR
		RGBA_Float16,
		RGBA_Float32,

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
		public TextureFormat Format { get; }

		/// <summary>
		/// Creates a new Texture2D object.
		/// </summary>
		public Texture2D(int width, int height, TextureFormat format, byte mipCount = 4)
		{
			Width = width;
			Height = height;
			Format = format;

			// Select the format to use GPU-side.
			DXGIFormat resourceFormat = format switch
			{
				TextureFormat.RGB => DXGIFormat.R8G8B8A8_UNorm,
				TextureFormat.RGBA => DXGIFormat.R8G8B8A8_UNorm,
				TextureFormat.RGBA_Float16 => DXGIFormat.R16G16B16A16_Float,
				TextureFormat.RGBA_Float32 => DXGIFormat.R32G32B32A32_Float,
				TextureFormat.BC1 => DXGIFormat.BC1_UNorm,
				TextureFormat.BC2 => DXGIFormat.BC2_UNorm,
				TextureFormat.BC3 => DXGIFormat.BC3_UNorm,
				TextureFormat.BC5 => DXGIFormat.BC5_UNorm,
				_ => throw new ArgumentOutOfRangeException()
			};

			Resource = new Texture(Width, Height, mipCount, resourceFormat);
		}

		/// <summary>
		/// Loads raw image data into the texture.
		/// </summary>
		/// <param name="data">Raw image data in this texture's format.</param>
		/// <param name="mipLevel">The mipmap level that's being set to.</param>
		/// <param name="generateMips">Automatically generate the other mipmaps? Only RGB/RGBA formats are supported.</param>
		public void SetPixelData(Span<byte> data, int mipLevel = 0, bool generateMips = false)
		{
			// D3D12 doesn't actually support RGB (24bpp), so we have to convert it to RGBA.
			if (Format == TextureFormat.RGB)
			{
				var convertedData = new byte[data.Length + (data.Length / 3)];
				for (int i = 0, j = 0; i < data.Length; i += 3, j += 4)
				{
					for (int k = 0; k < 3; k++)
					{
						convertedData[j + k] = data[i + k];
					}

					convertedData[j + 3] = 255;
				}

				data = convertedData.AsSpan();
			}

			// Create resource and upload texture data.
			Renderer.DefaultCommandList.UploadTexture(Resource, data, mipLevel);

			// Generate mipmaps if requested.
			if (generateMips && Format == TextureFormat.RGBA || Format == TextureFormat.RGB)
			{
				Renderer.DefaultCommandList.GenerateMips(Resource);
			}
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
				for (int j = 0; j < 4; j++)
				{
					data[i + j] = (byte)(color[j] * 255);
				}
			};

			Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA, 1);
			texture.SetPixelData(data);
			return texture;
		}

		public static Texture2D White = FromColor(Color.White);
		public static Texture2D Black = FromColor(Color.Black);
		public static Texture2D Normal = FromColor(new Color(0.5f, 0.5f, 1));
		public static Texture2D Purple = FromColor(new Color(1, 0, 1));
	}
}