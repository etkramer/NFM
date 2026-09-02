using System;
using System.Runtime.CompilerServices;

namespace SourceEngine.Formats.Studio;

/// <summary>A parsed .dx90.vtx - the strip data that turns pool vertices into triangles.</summary>
/// <remarks>Every offset is relative to the struct holding it, so each level carries its own base.</remarks>
public sealed class MeshStripFile
{
	public int Checksum { get; private set; }
	public VtxBodyPartInfo[] BodyParts { get; private set; }

	public static MeshStripFile Read(string path, ReadOnlySpan<byte> file)
	{
		VtxFileHeader header = StudioReader.Read<VtxFileHeader>(file, 0);

		if (header.Version != StudioMagic.VtxVersion)
		{
			throw new StudioFormatException(path, $"unsupported VTX version {header.Version}");
		}

		VtxBodyPartInfo[] bodyParts = new VtxBodyPartInfo[header.NumBodyParts];

		for (int i = 0; i < bodyParts.Length; i++)
		{
			int partBase = header.BodyPartOffset + (i * Unsafe.SizeOf<VtxBodyPart>());
			VtxBodyPart part = StudioReader.Read<VtxBodyPart>(file, partBase);

			VtxModelInfo[] models = new VtxModelInfo[part.NumModels];
			for (int j = 0; j < models.Length; j++)
			{
				int modelBase = partBase + part.ModelOffset + (j * Unsafe.SizeOf<VtxModel>());
				models[j] = new VtxModelInfo() { Meshes = ReadLod0(path, file, modelBase) };
			}

			bodyParts[i] = new VtxBodyPartInfo() { Models = models };
		}

		return new MeshStripFile() { Checksum = header.CheckSum, BodyParts = bodyParts };
	}

	private static VtxMeshInfo[] ReadLod0(string path, ReadOnlySpan<byte> file, int modelBase)
	{
		VtxModel model = StudioReader.Read<VtxModel>(file, modelBase);

		if (model.NumLODs == 0)
		{
			return [];
		}

		int lodBase = modelBase + model.LodOffset;
		VtxModelLOD lod = StudioReader.Read<VtxModelLOD>(file, lodBase);

		VtxMeshInfo[] meshes = new VtxMeshInfo[lod.NumMeshes];

		for (int i = 0; i < meshes.Length; i++)
		{
			int meshBase = lodBase + lod.MeshOffset + (i * Unsafe.SizeOf<VtxMesh>());
			VtxMesh mesh = StudioReader.Read<VtxMesh>(file, meshBase);

			VtxStripGroupInfo[] groups = new VtxStripGroupInfo[mesh.NumStripGroups];
			for (int j = 0; j < groups.Length; j++)
			{
				int groupBase = meshBase + mesh.StripGroupHeaderOffset + (j * Unsafe.SizeOf<VtxStripGroup>());
				groups[j] = ReadStripGroup(path, file, groupBase);
			}

			meshes[i] = new VtxMeshInfo() { StripGroups = groups };
		}

		return meshes;
	}

	private static VtxStripGroupInfo ReadStripGroup(string path, ReadOnlySpan<byte> file, int groupBase)
	{
		VtxStripGroup group = StudioReader.Read<VtxStripGroup>(file, groupBase);

		VtxStrip[] strips = StudioReader.ReadArray<VtxStrip>(file, groupBase + group.StripOffset, group.NumStrips).ToArray();

		// Sum check catches the newer, wider VTX layout.
		if (strips.Sum(o => o.NumVerts) != group.NumVerts || strips.Sum(o => o.NumIndices) != group.NumIndices)
		{
			throw new StudioFormatException(path, "strip group doesn't match its strips - unexpected VTX layout");
		}

		return new VtxStripGroupInfo()
		{
			Vertices = StudioReader.ReadArray<VtxVertex>(file, groupBase + group.VertOffset, group.NumVerts).ToArray(),
			Indices = StudioReader.ReadArray<ushort>(file, groupBase + group.IndexOffset, group.NumIndices).ToArray(),
			Strips = strips
		};
	}
}

public sealed class VtxBodyPartInfo
{
	public VtxModelInfo[] Models;
}

public sealed class VtxModelInfo
{
	public VtxMeshInfo[] Meshes;
}

public sealed class VtxMeshInfo
{
	public VtxStripGroupInfo[] StripGroups;
}

public sealed class VtxStripGroupInfo
{
	public VtxVertex[] Vertices;
	public ushort[] Indices;
	public VtxStrip[] Strips;
}
