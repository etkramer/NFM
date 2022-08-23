using System;
using Engine;
using Engine.Resources;
using Engine.Mathematics;
using Assimp;
using AI = Assimp;
using ModelPart = Engine.Resources.ModelPart;
using Material = Engine.Resources.Material;
using Engine.Core;
using StbImageSharp;

namespace Basic.Loaders
{
	public class GLTFLoader : AssetLoader<Model>
	{
		public string Path;

		public GLTFLoader(string path)
		{
			Path = path;
		}

		public override async Task<Model> Load()
		{
			// Load GLTF file.
			AI.AssimpContext importContext = new AssimpContext();
			AI.Scene importScene = importContext.ImportFile(Path, PostProcessSteps.PreTransformVertices);

			// Create embedded materials.
			Material[] materials = new Material[importScene.MaterialCount];
			for (int i = 0; i < importScene.MaterialCount; i++)
			{
				Shader shader = await Asset.GetAsync<Shader>("Shaders/Generic");
				Material material = new Material(shader);

				materials[i] = material;
			}

			// Create submeshes.
			List<Submesh> submeshes = new();
			foreach (var importMesh in importScene.Meshes)
			{
				List<Vector3> vertices = new();
				List<Vector3> normals = new();
				List<uint> indices = new();

				// Interpret vertices.
				for (int j = 0; j < importMesh.VertexCount; j++)
				{
					vertices.Add(new(importMesh.Vertices[j].X, importMesh.Vertices[j].Y, importMesh.Vertices[j].Z));
				}

				// Interpret normals.
				for (int j = 0; j < importMesh.VertexCount; j++)
				{
					normals.Add(new(importMesh.Normals[j].X, importMesh.Normals[j].Y, importMesh.Normals[j].Z));
				}

				// Interpret indices.
				indices.AddRange(importMesh.GetUnsignedIndices());

				// Create submesh.
				Submesh submesh = new Submesh();
				submesh.Material = materials[importMesh.MaterialIndex];
				submesh.Vertices = vertices.ToArray();
				submesh.Normals = normals.ToArray();
				submesh.Triangles = indices.ToArray();
				submeshes.Add(submesh);
			}

			// Create model.
			Model model = new Model();
			model.Parts = new[] { new ModelPart()
			{
				Submeshes = submeshes.ToArray()
			}};

			return model;
		}
	}
}
