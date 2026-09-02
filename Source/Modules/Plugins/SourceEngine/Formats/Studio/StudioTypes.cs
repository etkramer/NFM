using System;
using System.Runtime.InteropServices;
using NFM.Mathematics;

namespace SourceEngine.Formats.Studio;

/// <summary>On-disk layouts from the 2007 Orange Box studio.h and optimize.h, packed and full-size.</summary>
public static class StudioMagic
{
	public const int Mdl = 0x54534449;      // 'IDST'
	public const int MdlSequence = 0x51534449; // 'IDSQ'
	public const int Vvd = 0x56534449;      // 'IDSV'
	public const int VvdThin = 0x56434449;  // 'IDCV'

	public const int MinVersion = 44;
	public const int MaxVersion = 48;

	public const int VtxVersion = 7;
	public const int VvdVersion = 4;

	public const int StaticProp = 0x10;

	public const byte StripIsTriList = 0x01;
	public const byte StripIsTriStrip = 0x02;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct StudioHeader
{
	public int Id;
	public int Version;
	public int Checksum;
	private fixed byte name[64];
	public int Length;

	public Vector3 EyePosition;
	public Vector3 IllumPosition;
	public Vector3 HullMin;
	public Vector3 HullMax;
	public Vector3 ViewBBMin;
	public Vector3 ViewBBMax;

	public int Flags;

	public int NumBones;
	public int BoneIndex;
	public int NumBoneControllers;
	public int BoneControllerIndex;
	public int NumHitboxSets;
	public int HitboxSetIndex;
	public int NumLocalAnim;
	public int LocalAnimIndex;
	public int NumLocalSeq;
	public int LocalSeqIndex;
	public int ActivityListVersion;
	public int EventsIndexed;
	public int NumTextures;
	public int TextureIndex;
	public int NumCdTextures;
	public int CdTextureIndex;
	public int NumSkinRef;
	public int NumSkinFamilies;
	public int SkinIndex;
	public int NumBodyParts;
	public int BodyPartIndex;

	public bool IsStaticProp => (Flags & StudioMagic.StaticProp) != 0;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct StudioBone
{
	public int SzNameIndex;
	public int Parent;
	private fixed int boneController[6];
	public Vector3 Pos;
	public Vector4 Quat;
	public Vector3 Rot;
	public Vector3 PosScale;
	public Vector3 RotScale;

	/// <summary>
	/// Model space to bone space at the bind pose - row-major 3x4 with column-vector semantics.
	/// </summary>
	public fixed float PoseToBone[12];

	private Vector4 qAlignment;
	public int Flags;
	private readonly int procType;
	private readonly int procIndex;
	private readonly int physicsBone;
	private readonly int surfacePropIdx;
	private readonly int contents;
	private fixed int unused[8];
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct StudioBodyPart
{
	public int SzNameIndex;
	public int NumModels;
	public int Base;
	public int ModelIndex;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct StudioModel
{
	public fixed byte Name[64];
	private readonly int type;
	private readonly float boundingRadius;
	public int NumMeshes;
	public int MeshIndex;
	public int NumVertices;

	/// <summary>Byte offset into the VVD vertex pool; divide by 48 for the first vertex.</summary>
	public int VertexIndex;

	/// <summary>Byte offset into the VVD tangent pool; divide by 16 for the first vertex.</summary>
	public int TangentsIndex;

	private readonly int numAttachments;
	private readonly int attachmentIndex;
	private readonly int numEyeballs;
	private readonly int eyeballIndex;
	private fixed int vertexData[2];
	private fixed int unused[8];
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct StudioMesh
{
	/// <summary>Index into the skin reference table, not directly into the texture list.</summary>
	public int Material;

	private readonly int modelIndex;
	public int NumVertices;

	/// <summary>Vertex offset within the owning model's block.</summary>
	public int VertexOffset;

	private readonly int numFlexes;
	private readonly int flexIndex;
	private readonly int materialType;
	private readonly int materialParam;
	private readonly int meshId;
	private Vector3 center;
	private readonly int vertexDataPtr;
	public fixed int NumLODVertexes[8];
	private fixed int unused[8];
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct StudioTexture
{
	public int SzNameIndex;
	private readonly int flags;
	private readonly int used;
	private readonly int unused1;
	private readonly int material;
	private readonly int clientMaterial;
	private fixed int unused[10];
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct VertexFileHeader
{
	public int Id;
	public int Version;
	public int Checksum;
	public int NumLODs;
	public fixed int NumLODVertexes[8];
	public int NumFixups;
	public int FixupTableStart;
	public int VertexDataStart;
	public int TangentDataStart;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct StudioVertex
{
	public fixed float Weight[3];
	public fixed sbyte Bone[3];
	public byte NumBones;
	public Vector3 Position;
	public Vector3 Normal;
	public Vector2 TexCoord;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct VertexFileFixup
{
	/// <summary>This run belongs to every LOD from 0 through this one.</summary>
	public int Lod;

	public int SourceVertexID;
	public int NumVertexes;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct VtxFileHeader
{
	public int Version;
	private readonly int vertCacheSize;
	private readonly ushort maxBonesPerStrip;
	private readonly ushort maxBonesPerTri;
	private readonly int maxBonesPerVert;
	public int CheckSum;
	public int NumLODs;
	private readonly int materialReplacementListOffset;
	public int NumBodyParts;
	public int BodyPartOffset;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct VtxBodyPart
{
	public int NumModels;
	public int ModelOffset;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct VtxModel
{
	public int NumLODs;
	public int LodOffset;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct VtxModelLOD
{
	public int NumMeshes;
	public int MeshOffset;
	private readonly float switchPoint;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct VtxMesh
{
	public int NumStripGroups;
	public int StripGroupHeaderOffset;
	private readonly byte flags;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct VtxStripGroup
{
	public int NumVerts;
	public int VertOffset;
	public int NumIndices;
	public int IndexOffset;
	public int NumStrips;
	public int StripOffset;
	private readonly byte flags;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct VtxStrip
{
	public int NumIndices;

	/// <summary>Element count into the parent strip group's index array, not a byte offset.</summary>
	public int IndexOffset;

	public int NumVerts;
	public int VertOffset;
	private readonly short numBones;
	public byte Flags;
	private readonly int numBoneStateChanges;
	private readonly int boneStateChangeOffset;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct VtxVertex
{
	private fixed byte boneWeightIndex[3];
	private readonly byte numBones;

	/// <summary>Vertex index relative to the owning mesh's block.</summary>
	public ushort OrigMeshVertID;

	private fixed sbyte boneID[3];
}
