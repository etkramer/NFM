using System;
using System.Runtime.CompilerServices;
using NFM;
using NFM.Common;
using NFM.Mathematics;
using NFM.Resources;
using SourceEngine.Filesystem;
using SourceEngine.Formats.Studio;
using Material = NFM.Resources.Material;
using Mesh = NFM.Resources.Mesh;

namespace SourceEngine.Loaders;

/// <summary>Loads a Source studiomodel from its .mdl, .vvd and .dx90.vtx siblings.</summary>
public class MdlLoader : ResourceLoader<Model>
{
	private static readonly string[] VtxExtensions = [".dx90.vtx", ".dx80.vtx", ".sw.vtx"];

	private readonly SourceFileSystem fileSystem;

	public string Path;

	public MdlLoader(SourceFileSystem fileSystem, string path)
	{
		this.fileSystem = fileSystem;
		Path = path;
	}

	/// <summary>Whether a file is a supported, complete studiomodel.</summary>
	public static bool IsSupported(string path)
	{
		if (!StudioModelFile.TryReadHeader(path, out StudioHeader header))
		{
			return false;
		}

		if (header.Id != StudioMagic.Mdl || header.Version < StudioMagic.MinVersion || header.Version > StudioMagic.MaxVersion)
		{
			return false;
		}

		// Animation-only models ship without geometry siblings.
		return File.Exists(System.IO.Path.ChangeExtension(path, ".vvd")) && FindVtx(path) is not null;
	}

	private static string FindVtx(string path)
	{
		string basePath = System.IO.Path.ChangeExtension(path, null);
		return VtxExtensions.Select(o => basePath + o).FirstOrDefault(File.Exists);
	}

	public override async Task<Model> Load()
	{
		byte[] mdlBytes = File.ReadAllBytes(Path);
		byte[] vvdBytes = File.ReadAllBytes(System.IO.Path.ChangeExtension(Path, ".vvd"));
		byte[] vtxBytes = File.ReadAllBytes(Guard.NotNull(FindVtx(Path)));

		StudioModelFile mdl = StudioModelFile.Read(Path, mdlBytes);
		VertexDataFile vvd = VertexDataFile.Read(Path, vvdBytes);
		MeshStripFile vtx = MeshStripFile.Read(Path, vtxBytes);

		// The three files only describe the same model if they came out of the same compile.
		if (vvd.Checksum != mdl.Header.Checksum || vtx.Checksum != mdl.Header.Checksum)
		{
			throw new StudioFormatException(Path, "checksum mismatch between .mdl, .vvd and .vtx");
		}
		if (vtx.BodyParts.Length != mdl.BodyParts.Length)
		{
			throw new StudioFormatException(Path, "bodypart count differs between .mdl and .vtx");
		}

		Dictionary<int, Material> materials = await LoadMaterials(mdl);

		Model model = new();
		bool isSkinned = !mdl.Header.IsStaticProp;

		if (isSkinned)
		{
			model.SetSkeleton(BuildSkeleton(mdl));
		}

		HashSet<string> names = [];

		for (int i = 0; i < mdl.BodyParts.Length; i++)
		{
			StudioBodyPartInfo part = mdl.BodyParts[i];

			for (int j = 0; j < part.Models.Length; j++)
			{
				// A bodypart model with no meshes is the "nothing equipped" bodygroup option.
				if (part.Models[j].Meshes.Length == 0)
				{
					continue;
				}

				Mesh[] meshes = BuildMeshes(mdl, vvd, vtx, materials, i, j, isSkinned);
				if (meshes.Length == 0)
				{
					continue;
				}

				model.AddMeshGroup(new MeshGroup()
				{
					Name = UniqueName(GetGroupName(part, j), names),
					Meshes = meshes,

					// Source's default body value selects the first model of every bodypart.
					IsVisible = j == 0
				});
			}
		}

		if (model.MeshGroups.Count == 0)
		{
			throw new StudioFormatException(Path, "no renderable geometry at LOD 0");
		}

		return model;
	}

	private Mesh[] BuildMeshes(StudioModelFile mdl, VertexDataFile vvd, MeshStripFile vtx, Dictionary<int, Material> materials, int partIndex, int modelIndex, bool isSkinned)
	{
		StudioModelInfo sourceModel = mdl.BodyParts[partIndex].Models[modelIndex];
		VtxMeshInfo[] vtxMeshes = vtx.BodyParts[partIndex].Models[modelIndex].Meshes;

		if (vtxMeshes.Length != sourceModel.Meshes.Length)
		{
			throw new StudioFormatException(Path, "mesh count differs between .mdl and .vtx");
		}

		int modelFirstVertex = sourceModel.Model.VertexIndex / Unsafe.SizeOf<StudioVertex>();
		Guard.Require(sourceModel.Model.TangentsIndex / Unsafe.SizeOf<Vector4>() == modelFirstVertex, "Studio vertex and tangent pools must stay in step");

		List<Mesh> meshes = [];
		List<(int Bone, float Weight)> influences = [];

		for (int k = 0; k < vtxMeshes.Length; k++)
		{
			StudioMesh sourceMesh = sourceModel.Meshes[k].Mesh;

			List<Vertex> vertices = [];
			List<VertexWeights> weights = [];
			List<uint> indices = [];

			foreach (VtxStripGroupInfo group in vtxMeshes[k].StripGroups)
			{
				int groupBase = vertices.Count;

				foreach (VtxVertex groupVertex in group.Vertices)
				{
					int poolIndex = modelFirstVertex + sourceMesh.VertexOffset + groupVertex.OrigMeshVertID;

					ref readonly StudioVertex sourceVertex = ref vvd.Vertex(poolIndex);
					ref readonly Vector4 tangent = ref vvd.Tangent(poolIndex);

					vertices.Add(new Vertex()
					{
						Position = sourceVertex.Position * SourceEnginePlugin.UnitScale,
						Normal = sourceVertex.Normal,
						Tangent = tangent,
						UV0 = sourceVertex.TexCoord,
						UV1 = sourceVertex.TexCoord
					});

					if (isSkinned)
					{
						weights.Add(BuildWeights(sourceVertex, influences));
					}
				}

				AppendIndices(group, groupBase, indices);
			}

			if (vertices.Count == 0)
			{
				continue;
			}

			meshes.Add(new Mesh()
			{
				Vertices = vertices.ToArray(),
				Indices = indices.ToArray(),
				Material = materials[sourceMesh.Material],
				Weights = isSkinned ? weights.ToArray() : null
			});
		}

		return meshes.ToArray();
	}

	/// <summary>Emits a strip group's triangles, wound counter-clockwise for NFM.</summary>
	private static void AppendIndices(VtxStripGroupInfo group, int groupBase, List<uint> indices)
	{
		foreach (VtxStrip strip in group.Strips)
		{
			ReadOnlySpan<ushort> span = group.Indices.AsSpan(strip.IndexOffset, strip.NumIndices);

			// Indices are already relative to the strip group.
			if ((strip.Flags & StudioMagic.StripIsTriStrip) != 0)
			{
				for (int i = 0; i + 2 < span.Length; i++)
				{
					uint a = span[i], b = span[i + 1], c = span[i + 2];
					if (a == b || b == c || a == c)
					{
						continue;
					}

					// Odd triangles in a strip are already reversed, so the flip lands the other way.
					indices.Add((uint)groupBase + a);
					indices.Add((uint)groupBase + ((i & 1) == 0 ? c : b));
					indices.Add((uint)groupBase + ((i & 1) == 0 ? b : c));
				}
			}
			else
			{
				for (int i = 0; i + 2 < span.Length; i += 3)
				{
					indices.Add((uint)(groupBase + span[i]));
					indices.Add((uint)(groupBase + span[i + 2]));
					indices.Add((uint)(groupBase + span[i + 1]));
				}
			}
		}
	}

	private static unsafe VertexWeights BuildWeights(in StudioVertex vertex, List<(int Bone, float Weight)> influences)
	{
		influences.Clear();

		for (int i = 0; i < vertex.NumBones && i < 3; i++)
		{
			if (vertex.Weight[i] > 0)
			{
				influences.Add((vertex.Bone[i], vertex.Weight[i]));
			}
		}

		return VertexWeights.FromInfluences(influences);
	}

	/// <summary>Bones come straight across - file index and skeleton index are the same.</summary>
	private static unsafe Skeleton BuildSkeleton(StudioModelFile mdl)
	{
		Bone[] bones = new Bone[mdl.Bones.Length];
		HashSet<string> names = [];

		for (int i = 0; i < bones.Length; i++)
		{
			ref StudioBone source = ref mdl.Bones[i].Bone;

			// Go through the matrix - rot's euler order isn't the one CreateRotation rebuilds.
			Rotation rotation = new(source.Quat.X, source.Quat.Y, source.Quat.Z, source.Quat.W);
			Vector3 euler = Matrix4.CreateFromQuaternion(rotation).ExtractEulerAngles();

			// poseToBone is already the inverse bind, in column-vector form.
			Matrix4 inverseBind = new Matrix4(
				new Vector4(source.PoseToBone[0], source.PoseToBone[1], source.PoseToBone[2], source.PoseToBone[3]),
				new Vector4(source.PoseToBone[4], source.PoseToBone[5], source.PoseToBone[6], source.PoseToBone[7]),
				new Vector4(source.PoseToBone[8], source.PoseToBone[9], source.PoseToBone[10], source.PoseToBone[11]),
				new Vector4(0, 0, 0, 1)).Transpose();

			// Scaling the translation of both the inverse bind and the pose keeps their product intact.
			inverseBind.Row3.Xyz *= SourceEnginePlugin.UnitScale;

			bones[i] = new Bone()
			{
				Name = UniqueName(mdl.Bones[i].Name, names),
				ParentIndex = source.Parent,
				Position = source.Pos * SourceEnginePlugin.UnitScale,
				Rotation = new Vector3(euler.X.ToDegrees(), euler.Y.ToDegrees(), euler.Z.ToDegrees()),
				Scale = Vector3.One,
				InverseBind = inverseBind
			};
		}

		return new Skeleton() { Bones = bones };
	}

	/// <summary>Resolves every referenced material up front, so shared ones are only awaited once.</summary>
	private async Task<Dictionary<int, Material>> LoadMaterials(StudioModelFile mdl)
	{
		int[] indices = mdl.BodyParts
			.SelectMany(o => o.Models)
			.SelectMany(o => o.Meshes)
			.Select(o => o.Mesh.Material)
			.Distinct()
			.ToArray();

		Material[] loaded = await Task.WhenAll(indices.Select(o => ResolveMaterial(mdl, o)));

		return indices.Zip(loaded).ToDictionary(o => o.First, o => o.Second);
	}

	private async Task<Material> ResolveMaterial(StudioModelFile mdl, int materialIndex)
	{
		string name = mdl.GetTextureName(materialIndex);

		if (!string.IsNullOrWhiteSpace(name))
		{
			// A name resolves against each cdmaterials entry in turn.
			foreach (string directory in mdl.CdMaterials.Append(string.Empty))
			{
				string gamePath = SourceFileSystem.Normalize($"materials/{directory}/{name}.vmt");

				if (fileSystem.TryResolveAsset(gamePath, out string assetPath)
					&& await Asset.LoadAsync<Material>(assetPath) is Material material)
				{
					return material;
				}
			}
		}

		Log.Warn($"{Path} references material '{name}', which doesn't resolve");
		return await MakePlaceholder();
	}

	private static async Task<Material> MakePlaceholder()
	{
		string shaderPath = SourceEnginePlugin.ShaderPath("VertexLitGeneric", BlendMode.Opaque, FaceMode.FrontOnly);
		Shader shader = await Asset.LoadAsync<Shader>($"{SourceEnginePlugin.ShaderMount}:/{shaderPath}");

		Material material = new(shader);
		material.SetTexture("BaseTexture", Texture2D.Purple);

		return material;
	}

	private static string GetGroupName(StudioBodyPartInfo part, int modelIndex)
	{
		if (part.Models.Length == 1)
		{
			return part.Name;
		}

		string name = System.IO.Path.GetFileNameWithoutExtension(part.Models[modelIndex].Name.Replace('\\', '/'));
		return string.IsNullOrWhiteSpace(name) ? $"{part.Name}_{modelIndex}" : $"{part.Name}_{name}";
	}

	// Group and bone names address nodes in the editor, so a duplicate would make one unreachable.
	private static string UniqueName(string name, HashSet<string> taken)
	{
		name = string.IsNullOrWhiteSpace(name) ? "unnamed" : name.Replace('/', '_');
		string unique = name;

		for (int i = 1; !taken.Add(unique); i++)
		{
			unique = $"{name}.{i}";
		}

		return unique;
	}
}
