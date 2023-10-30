using System;
using System.Collections;
using System.Runtime.InteropServices;
using NFM;
using NFM.Resources;
using NFM.Mathematics;
using Asset = NFM.Resources.Asset;
using Material = NFM.Resources.Material;
using Mesh = NFM.Resources.Mesh;
using Assimp;
using StbiSharp;
using NFM.Common;
using NFM.World;
using AI = Assimp;

namespace GLTF.Loaders;

public class GLTFLoader : ResourceLoader<Model>
{
	public string Path;

	public GLTFLoader(string path)
	{
		Path = path;
	}

	public override async Task<Model> Load()
	{
		// Load GLTF model from file.
		AssimpContext importer = new AssimpContext();
		var sourceModel = importer.ImportFile(Path, PostProcessSteps.None);

		// Load textures from GLTF
		var textures = new Texture2D[sourceModel.TextureCount];
		Parallel.For(0, sourceModel.TextureCount, (i) =>
		{
			var sourceTexture = sourceModel.Textures[i];
			Guard.Require(sourceTexture.HasCompressedData);

			using (StbiImage sourceImage = Stbi.LoadFromMemory(sourceTexture.CompressedData.AsSpan(), 4))
			{
				Texture2D texture = new Texture2D(sourceImage.Width, sourceImage.Height, TextureFormat.RGBA8, 4);
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
			Shader shader = await Asset.LoadAsync<Shader>("USER:/Shaders/Opaque.hlsl");

			// Load textures
			sourceMaterial.GetMaterialTexture(TextureType.Diffuse, 0, out var baseColor);
			sourceMaterial.GetMaterialTexture(TextureType.Normals, 0, out var normal);
			sourceMaterial.GetMaterialTexture(TextureType.Unknown, 0, out var orm);

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

			materials[i] = material;
		}

		// Create model for NFM
		Model model = new Model();

		VisitMeshNodes(sourceModel.RootNode, Matrix4.Identity, (node) =>
		{
			// Get node transform as Z-up
			var worldTransform = node.Item2 * Matrix4.CreateRotation(new(90, 0, 0));

			for (int i = 0; i < node.Item1.MeshCount; i++)
			{
				var sourceMesh = sourceModel.Meshes[node.Item1.MeshIndices[i]];

				// Reformat vertices
				Vertex[] vertices = new Vertex[sourceMesh.Vertices.Count];
				for (int j = 0; j < sourceMesh.Vertices.Count; j++)
				{
					var position = (new Vector4(sourceMesh.Vertices[j].X, sourceMesh.Vertices[j].Y, sourceMesh.Vertices[j].Z, 1) * worldTransform).Xyz;
					var normal = (new Vector4(sourceMesh.Normals[j].X, sourceMesh.Normals[j].Y, sourceMesh.Normals[j].Z, 1) * worldTransform).Xyz;
					var uv0 = sourceMesh.TextureCoordinateChannels[0][j];

					unsafe
					{
						vertices[j] = new Vertex();
						vertices[j].Position = position;
						vertices[j].Normal = normal;
						vertices[j].UV0 = (*(Vector2*)&uv0) * new Vector2(1, -1);
					}
				}

				// Create mesh
				var mesh = new Mesh(sourceMesh.Name ?? "unnamed");
				mesh.SetVertices(vertices);
				mesh.SetIndices(sourceMesh.GetUnsignedIndices());
				mesh.SetMaterial(materials[sourceMesh.MaterialIndex]);

				// Add to new mesh (body) group
				model.AddMesh(mesh);
			}
		});

		return model;
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
