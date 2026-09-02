using System;
using System.Collections;
using System.Runtime.InteropServices;
using NFM;
using NFM.Resources;
using NFM.Mathematics;
using Asset = NFM.Resources.Asset;
using Material = NFM.Resources.Material;
using Mesh = NFM.Resources.Mesh;
using Bone = NFM.Resources.Bone;
using Assimp;
using StbiSharp;
using NFM.Common;
using NFM.World;
using AI = Assimp;

namespace GLTF.Loaders;

public class GLTFLoader : ResourceLoader<Model>
{
	// GLTF is Y-up, NFM is Z-up.
	private static readonly Matrix4 ZUp = Matrix4.CreateRotation(new(90, 0, 0));

	public string Path;

	public GLTFLoader(string path)
	{
		Path = path;
	}

	public override async Task<Model> Load()
	{
		// Load GLTF model from file.
		var importer = new AssimpContext();
		var sourceModel = importer.ImportFile(Path, PostProcessSteps.CalculateTangentSpace);

		// Load textures from GLTF
		var textures = new Texture2D[sourceModel.TextureCount];
		Parallel.For(0, sourceModel.TextureCount, (i) =>
		{
			var sourceTexture = sourceModel.Textures[i];
			Guard.Require(sourceTexture.HasCompressedData);

			using (StbiImage sourceImage = Stbi.LoadFromMemory(sourceTexture.CompressedData.AsSpan(), 4))
			{
				var texture = new Texture2D(sourceImage.Width, sourceImage.Height, TextureFormat.RGBA8, 4);
				texture.SetPixelData(sourceImage.Data, 0, true);

				textures[i] = texture;
			}
		});

		// Load materials from GLTF
		Material[] materials = new Material[sourceModel.MaterialCount];
		for (int i = 0; i < sourceModel.MaterialCount; i++)
		{
			var sourceMaterial = sourceModel.Materials[i];

			// Determine shader
			var shader = await Asset.LoadAsync<Shader>("USER:/Shaders/Opaque.hlsl");

			// Load textures
			sourceMaterial.GetMaterialTexture(TextureType.Diffuse, 0, out var baseColor);
			sourceMaterial.GetMaterialTexture(TextureType.Normals, 0, out var normal);
			sourceMaterial.GetMaterialTexture(TextureType.Unknown, 0, out var orm);
			sourceMaterial.GetMaterialTexture(TextureType.Emissive, 0, out var emissive);

			// Create material from channels
			Material material = new Material(shader);
			if (!string.IsNullOrEmpty(baseColor.FilePath))
			{
				int index = int.Parse(baseColor.FilePath.Split('*')[1]);
				material.SetTexture("BaseColor", textures[index]);
			}
			if (!string.IsNullOrEmpty(normal.FilePath))
			{
				int index = int.Parse(normal.FilePath.Split('*')[1]);
				material.SetTexture("Normal", textures[index]);
			}
			if (!string.IsNullOrEmpty(orm.FilePath))
			{
				int index = int.Parse(orm.FilePath.Split('*')[1]);
				material.SetTexture("ORM", textures[index]);
			}
			if (!string.IsNullOrEmpty(emissive.FilePath))
			{
				int index = int.Parse(emissive.FilePath.Split('*')[1]);
				material.SetTexture("Emissive", textures[index]);
			}

			material.SetColor("BaseColorFactor", sourceMaterial.HasColorDiffuse ? ToColor(sourceMaterial.ColorDiffuse) : Color.White);
			material.SetColor("EmissiveFactor", sourceMaterial.HasColorEmissive ? ToColor(sourceMaterial.ColorEmissive) : Color.Black);
			material.SetFloat("RoughnessFactor", GetFactor(sourceMaterial, "roughnessFactor"));
			material.SetFloat("MetallicFactor", GetFactor(sourceMaterial, "metallicFactor"));

			materials[i] = material;
		}

		// Create model for NFM
		Model model = new Model();

		var skeleton = BuildSkeleton(sourceModel, out var boneIndices);
		if (skeleton != null)
		{
			model.SetSkeleton(skeleton);
		}

		VisitMeshNodes(sourceModel.RootNode, Matrix4.Identity, (node) =>
		{
			for (int i = 0; i < node.Item1.MeshCount; i++)
			{
				var sourceMesh = sourceModel.Meshes[node.Item1.MeshIndices[i]];
				Guard.Require(sourceMesh.HasNormals && sourceMesh.HasTangentBasis);

				// Skinned geometry stays in mesh space - the bone chain carries it the rest of the way,
				// Z-up correction included. Everything else gets its node transform baked in.
				bool isSkinned = skeleton != null && sourceMesh.HasBones;
				var worldTransform = isSkinned ? Matrix4.Identity : node.Item2 * ZUp;

				// Reformat vertices
				Vertex[] vertices = new Vertex[sourceMesh.Vertices.Count];
				for (int j = 0; j < sourceMesh.Vertices.Count; j++)
				{
					var position = (new Vector4(sourceMesh.Vertices[j].X, sourceMesh.Vertices[j].Y, sourceMesh.Vertices[j].Z, 1) * worldTransform).Xyz;
					var normal = TransformDirection(sourceMesh.Normals[j], worldTransform);
					var tangent = TransformDirection(sourceMesh.Tangents[j], worldTransform);
					var bitangent = TransformDirection(sourceMesh.BiTangents[j], worldTransform);
					var uv0 = sourceMesh.TextureCoordinateChannels[0][j];

					// V is flipped below, which flips the bitangent along with it.
					float handedness = -MathF.Sign(Vector3.Dot(Vector3.Cross(normal, tangent), bitangent));

					unsafe
					{
						vertices[j] = new Vertex();
						vertices[j].Position = position;
						vertices[j].Normal = normal;
						vertices[j].Tangent = new Vector4(tangent, handedness);
						vertices[j].UV0 = (*(Vector2*)&uv0) * new Vector2(1, -1);
					}
				}

				// Create mesh
                var mesh = new Mesh()
                {
                    Vertices = vertices,
                    Indices = sourceMesh.GetUnsignedIndices(),
                    Material = materials[sourceMesh.MaterialIndex],
                    Weights = isSkinned ? BuildWeights(sourceMesh, boneIndices, vertices.Length) : null
                };

				// Add to new mesh (body) group
				model.AddMeshGroup(mesh, sourceMesh.Name ?? "unnamed");
			}
		});

		return model;
	}

	/// <summary>
	/// Builds a skeleton from every bone the model's meshes reference, or null if none do. Bones come
	/// back in tree order, so a parent always precedes its children.
	/// </summary>
	private static Skeleton BuildSkeleton(AI.Scene sourceModel, out Dictionary<string, int> indices)
	{
		indices = [];
		var boneIndices = indices;

		// Offset matrices are per-mesh, but a bone shared between meshes carries the same one.
		var offsets = new Dictionary<string, Matrix4>();
		foreach (var sourceMesh in sourceModel.Meshes)
		{
			foreach (var bone in sourceMesh.Bones)
			{
				offsets[bone.Name] = ToMatrix(bone.OffsetMatrix);
			}
		}

		if (offsets.Count == 0)
		{
			return null;
		}

		// Ancestors of a bone are kept too - they carry transforms the chain would otherwise lose.
		var included = new HashSet<AI.Node>();
		foreach (var name in offsets.Keys)
		{
			for (var node = sourceModel.RootNode.FindNode(name); node != null && node != sourceModel.RootNode; node = node.Parent)
			{
				included.Add(node);
			}
		}

		var bones = new List<Bone>();
		var names = new HashSet<string>();

		void Visit(AI.Node node, Matrix4 parentWorld, int parentIndex)
		{
			var world = ToMatrix(node.Transform) * parentWorld;
			var index = parentIndex;

			if (included.Contains(node))
			{
				// A root bone folds in everything above it, Z-up correction included; the rest of the
				// chain inherits that and keeps its own local transform.
				var local = parentIndex < 0 ? world * ZUp : ToMatrix(node.Transform);
				var euler = local.ExtractEulerAngles();

				index = bones.Count;
				boneIndices[node.Name] = index;

				bones.Add(new Bone()
				{
					Name = UniqueName(node.Name, names),
					ParentIndex = parentIndex,
					Position = local.ExtractTranslation(),
					Rotation = new Vector3(euler.X.ToDegrees(), euler.Y.ToDegrees(), euler.Z.ToDegrees()),
					Scale = local.ExtractScale(),
					InverseBind = offsets.GetValueOrDefault(node.Name, Matrix4.Identity)
				});
			}

			foreach (var child in node.Children)
			{
				Visit(child, world, index);
			}
		}

		Visit(sourceModel.RootNode, Matrix4.Identity, -1);

		return new Skeleton() { Bones = bones.ToArray() };
	}

	/// <summary>
	/// Collects each vertex's bone influences, resolved against the skeleton's indices.
	/// </summary>
	private static VertexWeights[] BuildWeights(AI.Mesh sourceMesh, Dictionary<string, int> boneIndices, int vertexCount)
	{
		var influences = new List<(int, float)>[vertexCount];

		foreach (var bone in sourceMesh.Bones)
		{
			if (!boneIndices.TryGetValue(bone.Name, out int index))
			{
				continue;
			}

			foreach (var weight in bone.VertexWeights)
			{
				(influences[weight.VertexID] ??= []).Add((index, weight.Weight));
			}
		}

		var weights = new VertexWeights[vertexCount];
		for (int i = 0; i < vertexCount; i++)
		{
			weights[i] = VertexWeights.FromInfluences(influences[i] ?? []);
		}

		return weights;
	}

	// Bone names address nodes in the editor, so a duplicate or a separator would make one unreachable.
	private static string UniqueName(string name, HashSet<string> taken)
	{
		name = string.IsNullOrEmpty(name) ? "Bone" : name.Replace('/', '_');
		string unique = name;

		for (int i = 1; !taken.Add(unique); i++)
		{
			unique = $"{name}.{i}";
		}

		return unique;
	}

	// Assimp moved the PBR factors out of the GLTF-specific namespace, so both keys are worth a look.
	private static float GetFactor(AI.Material material, string name, float fallback = 1)
	{
		var property = material.GetNonTextureProperty($"$mat.gltf.pbrMetallicRoughness.{name}")
			?? material.GetNonTextureProperty($"$mat.{name}");

		return property == null ? fallback : property.GetFloatValue();
	}

	private static Color ToColor(AI.Color4D color) => new Color(color.R, color.G, color.B, color.A);

	// Assimp stores column-vector matrices, which transpose into the row-vector form NFM uses.
	private static unsafe Matrix4 ToMatrix(AI.Matrix4x4 matrix) => (*(Matrix4*)&matrix).Transpose();

	// Directions carry no translation, so W stays zero.
	private static Vector3 TransformDirection(AI.Vector3D direction, Matrix4 transform)
	{
		return (new Vector4(direction.X, direction.Y, direction.Z, 0) * transform).Xyz.Normalized();
	}

	private unsafe void VisitMeshNodes(AI.Node baseNode, Matrix4 baseTransform, Action<(AI.Node, Matrix4)> visit)
	{
		var nodeTransform = baseNode.Transform;
		baseTransform = baseTransform * (*(Matrix4*)&nodeTransform);

		if (baseNode.HasMeshes)
		{
			visit((baseNode, baseTransform.Transpose()));
		}

		foreach (var node in baseNode.Children)
		{
			VisitMeshNodes(node, baseTransform, visit);
		}
	}
}
