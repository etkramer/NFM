using System;
using NFM.GPU;
using NFM.Rendering;
using DXGIFormat = Vortice.DXGI.Format;

namespace NFM.Resources
{
	public static class TextureFormatExtensions
	{
		public static bool IsCompressed(this TextureFormat format)
		{
			return format switch
			{
				TextureFormat.BC1 => true,
				TextureFormat.BC2 => true,
				TextureFormat.BC3 => true,
				TextureFormat.BC5 => true,
				TextureFormat.BC7 => true,
				_ => false
			};
		}

		public static bool IsHDR(this TextureFormat format)
		{
			return format switch
			{
				TextureFormat.RGBA16F => true,
				TextureFormat.RGBA32F => true,
				_ => false
			};
		}
	}

	public enum TextureFormat
	{
		RGB8 = 0,
		RGBA8,

		// Floating-point HDR
		RGBA16F,
		RGBA32F,

		// Block Compression
		BC1,
		BC2,
		BC3,
		BC5,
		BC7,

		// DXT aliases
		DXT1 = BC1,
		DXT3 = BC2,
		DXT5 = BC3,
	}

	public sealed class Texture2D : GameResource
	{
		public Texture D3DResource { get; private set; }

		public int Width { get; }
		public int Height { get; }
		public byte MipCount => D3DResource.MipmapCount;
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
				TextureFormat.RGB8 => DXGIFormat.R8G8B8A8_UNorm,
				TextureFormat.RGBA8 => DXGIFormat.R8G8B8A8_UNorm,
				TextureFormat.RGBA16F => DXGIFormat.R16G16B16A16_Float,
				TextureFormat.RGBA32F => DXGIFormat.R32G32B32A32_Float,
				TextureFormat.BC1 => DXGIFormat.BC1_UNorm,
				TextureFormat.BC2 => DXGIFormat.BC2_UNorm,
				TextureFormat.BC3 => DXGIFormat.BC3_UNorm,
				TextureFormat.BC5 => DXGIFormat.BC5_UNorm,
				TextureFormat.BC7 => DXGIFormat.BC7_UNorm,
				_ => throw new ArgumentOutOfRangeException()
			};

			D3DResource = new Texture(Width, Height, mipCount, resourceFormat);
		}

		/// <summary>
		/// Loads raw image data into the texture.
		/// </summary>
		/// <param name="data">Raw image data in this texture's format.</param>
		/// <param name="mipLevel">The mipmap level that's being set to.</param>
		/// <param name="generateMips">Automatically generate the other mipmaps? Only RGB/RGBA formats are supported.</param>
		public void SetPixelData(ReadOnlySpan<byte> data, int mipLevel = 0, bool generateMips = false)
		{
			// D3D12 doesn't actually support RGB (24bpp), so we have to convert it to RGBA.
			if (Format == TextureFormat.RGB8)
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
			Renderer.DefaultCommandList.UploadTexture(D3DResource, data, mipLevel);

			// Generate mipmaps if requested.
			if (generateMips && Format == TextureFormat.RGBA8 || Format == TextureFormat.RGB8)
			{
				Renderer.DefaultCommandList.GenerateMips(D3DResource);
			}
		}

		public override void Dispose()
		{
			D3DResource.Dispose();
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

			Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA8, 1);
			texture.SetPixelData(data);
			return texture;
		}

		public static Texture2D White = FromColor(Color.White);
		public static Texture2D Black = FromColor(Color.Black);
		public static Texture2D Purple = FromColor(Color.Purple);
		public static Texture2D Normal = FromColor(new Color(0.5f, 0.5f, 1));
	}
}