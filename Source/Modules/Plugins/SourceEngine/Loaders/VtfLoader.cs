using System;
using NFM;
using NFM.Common;
using NFM.Resources;
using Sledge.Formats.Texture.Vtf;

namespace SourceEngine.Loaders;

/// <summary>Loads a VTF - block-compressed images pass through, everything else is BGRA swizzled.</summary>
public class VtfLoader : ResourceLoader<Texture2D>
{
	public string Path;

	public VtfLoader(string path)
	{
		Path = path;
	}

	public override Task<Texture2D> Load()
	{
		VtfFile file;
		using (var stream = File.OpenRead(Path))
		{
			file = new VtfFile(stream);
		}

		// Mipmap numbering isn't a documented direction, so order by size instead.
		var mips = file.Images
			.Where(o => o.Frame == 0 && o.Face == 0 && o.Slice == 0)
			.OrderByDescending(o => o.Width * o.Height)
			.ToArray();

		Guard.Require(mips.Length > 0, $"{Path} has no image data");

		VtfImage largest = mips[0];
		TextureFormat format = GetFormat(largest.Format);

		Texture2D texture = new(largest.Width, largest.Height, format, (byte)mips.Length);

		for (int i = 0; i < mips.Length; i++)
		{
			texture.SetPixelData(format.IsCompressed() ? mips[i].Data : ToRgba(mips[i]), i);
		}

		return Task.FromResult(texture);
	}

	private static TextureFormat GetFormat(VtfImageFormat format)
	{
		return format switch
		{
			VtfImageFormat.Dxt1 => TextureFormat.BC1,
			VtfImageFormat.Dxt1Onebitalpha => TextureFormat.BC1,
			VtfImageFormat.Dxt3 => TextureFormat.BC2,
			VtfImageFormat.Dxt5 => TextureFormat.BC3,
			_ => TextureFormat.RGBA8
		};
	}

	private static byte[] ToRgba(VtfImage image)
	{
		byte[] data = image.GetBgra32Data();

		for (int i = 0; i < data.Length; i += 4)
		{
			(data[i], data[i + 2]) = (data[i + 2], data[i]);
		}

		return data;
	}
}
