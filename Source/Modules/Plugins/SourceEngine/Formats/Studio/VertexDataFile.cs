using System;
using System.Runtime.CompilerServices;
using NFM.Mathematics;

namespace SourceEngine.Formats.Studio;

/// <summary>A parsed .vvd - the vertex and tangent pools, plus the LOD-0 permutation.</summary>
public sealed class VertexDataFile
{
	private StudioVertex[] vertices;
	private Vector4[] tangents;
	private int[] lod0Map;

	public int Checksum { get; private set; }

	/// <summary>Number of vertices addressable at LOD 0.</summary>
	public int Count => lod0Map.Length;

	public static VertexDataFile Read(string path, ReadOnlySpan<byte> file)
	{
		VertexFileHeader header = StudioReader.Read<VertexFileHeader>(file, 0);

		if (header.Id == StudioMagic.VvdThin)
		{
			throw new StudioFormatException(path, "thin/runtime vertex data");
		}
		if (header.Id != StudioMagic.Vvd || header.Version != StudioMagic.VvdVersion)
		{
			throw new StudioFormatException(path, $"not a v{StudioMagic.VvdVersion} vertex file");
		}

		int poolSize;
		unsafe
		{
			poolSize = header.NumLODVertexes[0];
		}

		return new VertexDataFile()
		{
			Checksum = header.Checksum,
			vertices = StudioReader.ReadArray<StudioVertex>(file, header.VertexDataStart, poolSize).ToArray(),
			tangents = StudioReader.ReadArray<Vector4>(file, header.TangentDataStart, poolSize).ToArray(),
			lod0Map = BuildLod0Map(file, header, poolSize)
		};
	}

	/// <summary>Maps a LOD-0 index to its pool slot - the fixup table is a permutation even at LOD 0.</summary>
	private static int[] BuildLod0Map(ReadOnlySpan<byte> file, in VertexFileHeader header, int poolSize)
	{
		if (header.NumFixups == 0)
		{
			int[] identity = new int[poolSize];
			for (int i = 0; i < identity.Length; i++)
			{
				identity[i] = i;
			}

			return identity;
		}

		ReadOnlySpan<VertexFileFixup> fixups = StudioReader.ReadArray<VertexFileFixup>(file, header.FixupTableStart, header.NumFixups);

		int[] map = new int[poolSize];
		int written = 0;

		foreach (VertexFileFixup fixup in fixups)
		{
			// Every run survives at LOD 0, so together they cover the whole pool.
			for (int i = 0; i < fixup.NumVertexes; i++)
			{
				map[written++] = fixup.SourceVertexID + i;
			}
		}

		return written == poolSize ? map : map[..written];
	}

	public ref readonly StudioVertex Vertex(int index) => ref vertices[lod0Map[index]];

	public ref readonly Vector4 Tangent(int index) => ref tangents[lod0Map[index]];
}
