using System;
using Engine;
using Engine.Resources;
using Engine.Mathematics;
using Assimp;
using AI = Assimp;
using ModelPart = Engine.Resources.ModelPart;
using Material = Engine.Resources.Material;
using System.Collections.Concurrent;
using Engine.Core;
using Mesh = Engine.Resources.Mesh;

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
				Shader shader = await Asset.GetAsync<Shader>("USER:Shaders/PBR");

				Material material = new Material(shader);
				material.SetInt("TempyIndex", i);

				materials[i] = material;
			}

			// Create submeshes.
			ConcurrentBag<Mesh> meshes = new();
			Parallel.ForEach(importScene.Meshes, (importMesh, state) =>
			{
				Vector3[] vertices = new Vector3[importMesh.VertexCount];
				Vector3[] normals = new Vector3[importMesh.VertexCount];

				// Create matrix for translating Y-up GLTF coords to Z-up engine coords.
				Matrix4 zUp = Matrix4.CreateRotation(new Vector3(-90, 180, 180));

				// Interpret vertices/normals.
				for (int i = 0; i < importMesh.VertexCount; i++)
				{
					vertices[i] = (new Vector4(importMesh.Vertices[i].X, importMesh.Vertices[i].Y, importMesh.Vertices[i].Z, 1) * zUp).Xyz;
					normals[i] = new(importMesh.Normals[i].X, importMesh.Normals[i].Y, importMesh.Normals[i].Z);
				}

				// Create submesh.
				Mesh submesh = new Mesh();
				submesh.SetMaterial(materials[importMesh.MaterialIndex]);
				submesh.SetVertices(vertices);
				submesh.SetNormals(normals);
				submesh.SetIndices(importMesh.GetUnsignedIndices());

				meshes.Add(submesh);
			});

			// Create model.
			Model model = new Model();
			model.Parts = new[] { new ModelPart()
			{
				Meshes = meshes.ToArray()
			}};

			return model;
		}
	}
}
