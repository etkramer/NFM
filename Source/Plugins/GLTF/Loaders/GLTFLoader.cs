using System;
using Engine;
using Engine.Resources;
using Engine.Mathematics;
using Assimp;
using AI = Assimp;
using ModelPart = Engine.Resources.ModelPart;
using Material = Engine.Resources.Material;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Basic.Loaders
{
	public class GLTFLoader : AssetLoader<Model>
	{
		static AssimpContext importContext = new AssimpContext();

		public string Path;

		public GLTFLoader(string path)
		{
			Path = path;
		}

		public override async Task<Model> Load()
		{
			// Load GLTF file.
			AI.Scene importScene = importContext.ImportFile(Path, PostProcessSteps.PreTransformVertices);

			// Create embedded materials.
			Material[] materials = new Material[importScene.MaterialCount];
			for (int i = 0; i < importScene.MaterialCount; i++)
			{
				Shader shader = await Asset.GetAsync<Shader>("Shaders/PBR");
				Material material = new Material(shader);

				materials[i] = material;
			}

			// Create submeshes.
			ConcurrentBag<Submesh> submeshes = new();
			Parallel.ForEach(importScene.Meshes, (importMesh, state) =>
			{
				Vector3[] vertices = new Vector3[importMesh.VertexCount];
				Vector3[] normals = new Vector3[importMesh.VertexCount];
				uint[] indices = new uint[importMesh.FaceCount * 3];

				// Interpret vertices/normals.
				for (int i = 0; i < importMesh.VertexCount; i++)
				{
					vertices[i] = new(importMesh.Vertices[i].X, importMesh.Vertices[i].Y, importMesh.Vertices[i].Z);
					normals[i] = new(importMesh.Normals[i].X, importMesh.Normals[i].Y, importMesh.Normals[i].Z);
				}

				// Interpret indices.
				indices = importMesh.GetUnsignedIndices();

				// Create submesh.
				Submesh submesh = new Submesh();
				submesh.Material = materials[importMesh.MaterialIndex];
				submesh.Vertices = vertices;
				submesh.Normals = normals;
				submesh.Triangles = indices;

				submeshes.Add(submesh);
			});

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
