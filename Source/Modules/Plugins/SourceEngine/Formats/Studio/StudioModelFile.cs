using System;
using System.Runtime.CompilerServices;
using NFM.Common;

namespace SourceEngine.Formats.Studio;

public sealed class StudioBoneInfo
{
	public string Name;
	public StudioBone Bone;
}

public sealed class StudioMeshInfo
{
	public StudioMesh Mesh;
}

public sealed class StudioModelInfo
{
	public string Name;
	public StudioModel Model;
	public StudioMeshInfo[] Meshes;
}

public sealed class StudioBodyPartInfo
{
	public string Name;
	public StudioModelInfo[] Models;
}

/// <summary>A parsed .mdl - skeleton, bodypart tree, and material name resolution.</summary>
public sealed class StudioModelFile
{
	public StudioHeader Header;
	public StudioBoneInfo[] Bones;
	public StudioBodyPartInfo[] BodyParts;
	public string[] CdMaterials;
	public string[] TextureNames;

	private short[] skinTable;
	private int numSkinRef;

	/// <summary>Reads the header only, to decide whether a file is worth registering.</summary>
	public static bool TryReadHeader(string path, out StudioHeader header)
	{
		header = default;

		try
		{
			Span<byte> buffer = stackalloc byte[Unsafe.SizeOf<StudioHeader>()];

			using (var stream = File.OpenRead(path))
			{
				if (stream.Read(buffer) != buffer.Length)
				{
					return false;
				}
			}

			header = StudioReader.Read<StudioHeader>(buffer, 0);
			return true;
		}
		catch (IOException)
		{
			return false;
		}
	}

	public static StudioModelFile Read(string path, ReadOnlySpan<byte> file)
	{
		StudioHeader header = StudioReader.Read<StudioHeader>(file, 0);

		if (header.Id != StudioMagic.Mdl)
		{
			throw new StudioFormatException(path, header.Id == StudioMagic.MdlSequence ? "sequence-only model" : "not a studiomodel");
		}
		if (header.Version < StudioMagic.MinVersion || header.Version > StudioMagic.MaxVersion)
		{
			throw new StudioFormatException(path, $"unsupported MDL version {header.Version}");
		}

		StudioModelFile mdl = new()
		{
			Header = header,
			numSkinRef = header.NumSkinRef,
			Bones = ReadBones(file, header),
			CdMaterials = ReadCdMaterials(file, header),
			TextureNames = ReadTextureNames(file, header),
			skinTable = StudioReader.ReadArray<short>(file, header.SkinIndex, header.NumSkinRef * header.NumSkinFamilies).ToArray()
		};

		mdl.BodyParts = ReadBodyParts(file, header);

		return mdl;
	}

	private static StudioBoneInfo[] ReadBones(ReadOnlySpan<byte> file, in StudioHeader header)
	{
		StudioBoneInfo[] bones = new StudioBoneInfo[header.NumBones];

		for (int i = 0; i < bones.Length; i++)
		{
			int offset = header.BoneIndex + (i * Unsafe.SizeOf<StudioBone>());
			StudioBone bone = StudioReader.Read<StudioBone>(file, offset);

			// The skeleton is built in file order, so a forward reference would break the parent chain.
			Guard.Require(bone.Parent < i, "Studio bone parents must precede their children");

			bones[i] = new StudioBoneInfo()
			{
				Name = StudioReader.ReadString(file, offset + bone.SzNameIndex),
				Bone = bone
			};
		}

		return bones;
	}

	// Unlike every other name in the format, these offsets are relative to the file, not the table.
	private static string[] ReadCdMaterials(ReadOnlySpan<byte> file, in StudioHeader header)
	{
		ReadOnlySpan<int> offsets = StudioReader.ReadArray<int>(file, header.CdTextureIndex, header.NumCdTextures);
		string[] paths = new string[offsets.Length];

		for (int i = 0; i < paths.Length; i++)
		{
			paths[i] = StudioReader.ReadString(file, offsets[i]);
		}

		return paths;
	}

	private static string[] ReadTextureNames(ReadOnlySpan<byte> file, in StudioHeader header)
	{
		string[] names = new string[header.NumTextures];

		for (int i = 0; i < names.Length; i++)
		{
			int offset = header.TextureIndex + (i * Unsafe.SizeOf<StudioTexture>());
			names[i] = StudioReader.ReadString(file, offset + StudioReader.Read<StudioTexture>(file, offset).SzNameIndex);
		}

		return names;
	}

	private static StudioBodyPartInfo[] ReadBodyParts(ReadOnlySpan<byte> file, in StudioHeader header)
	{
		StudioBodyPartInfo[] bodyParts = new StudioBodyPartInfo[header.NumBodyParts];

		for (int i = 0; i < bodyParts.Length; i++)
		{
			int partOffset = header.BodyPartIndex + (i * Unsafe.SizeOf<StudioBodyPart>());
			StudioBodyPart part = StudioReader.Read<StudioBodyPart>(file, partOffset);

			StudioModelInfo[] models = new StudioModelInfo[part.NumModels];
			for (int j = 0; j < models.Length; j++)
			{
				int modelOffset = partOffset + part.ModelIndex + (j * Unsafe.SizeOf<StudioModel>());
				StudioModel model = StudioReader.Read<StudioModel>(file, modelOffset);

				StudioMeshInfo[] meshes = new StudioMeshInfo[model.NumMeshes];
				for (int k = 0; k < meshes.Length; k++)
				{
					int meshOffset = modelOffset + model.MeshIndex + (k * Unsafe.SizeOf<StudioMesh>());
					meshes[k] = new StudioMeshInfo() { Mesh = StudioReader.Read<StudioMesh>(file, meshOffset) };
				}

				models[j] = new StudioModelInfo()
				{
					Name = StudioReader.ReadString(file, modelOffset),
					Model = model,
					Meshes = meshes
				};
			}

			bodyParts[i] = new StudioBodyPartInfo()
			{
				Name = StudioReader.ReadString(file, partOffset + part.SzNameIndex),
				Models = models
			};
		}

		return bodyParts;
	}

	/// <summary>
	/// Resolves a mesh's material index through the default skin family to a texture name.
	/// </summary>
	public string GetTextureName(int materialIndex)
	{
		int index = materialIndex;

		if (skinTable.Length > 0 && materialIndex >= 0 && materialIndex < numSkinRef)
		{
			index = skinTable[materialIndex];
		}

		return index >= 0 && index < TextureNames.Length ? TextureNames[index] : null;
	}
}
