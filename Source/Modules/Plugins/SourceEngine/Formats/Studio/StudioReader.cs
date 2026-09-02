using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace SourceEngine.Formats.Studio;

public class StudioFormatException : Exception
{
	public StudioFormatException(string path, string message) : base($"{path}: {message}") {}
}

/// <summary>Struct reads over a file held in memory - slicing gives free bounds checks.</summary>
public static class StudioReader
{
	public static T Read<T>(ReadOnlySpan<byte> file, int offset) where T : unmanaged
	{
		return MemoryMarshal.Read<T>(file.Slice(offset, Unsafe.SizeOf<T>()));
	}

	public static ReadOnlySpan<T> ReadArray<T>(ReadOnlySpan<byte> file, int offset, int count) where T : unmanaged
	{
		return MemoryMarshal.Cast<byte, T>(file.Slice(offset, count * Unsafe.SizeOf<T>()));
	}

	/// <summary>Reads a null-terminated string, as every name in these formats is stored.</summary>
	public static string ReadString(ReadOnlySpan<byte> file, int offset)
	{
		if (offset <= 0 || offset >= file.Length)
		{
			return string.Empty;
		}

		ReadOnlySpan<byte> rest = file[offset..];
		int end = rest.IndexOf((byte)0);

		return Encoding.ASCII.GetString(end < 0 ? rest : rest[..end]);
	}
}
