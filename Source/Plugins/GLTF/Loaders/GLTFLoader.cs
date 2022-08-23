using System;
using Engine;
using Engine.Resources;
using Engine.Mathematics;
using Assimp;
using AI = Assimp;
using ModelPart = Engine.Resources.ModelPart;
using Face = Engine.Resources.Face;
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

			uint indexOffset = 0;
			List<Vector3> positions = new();
			List<Vector3> normals = new();
			List<Face> faces = new();

			// Create embedded materials.
			Material[] materials = new Material[importScene.MaterialCount];
			for (int i = 0; i < importScene.MaterialCount; i++)
			{
				Shader shader = await Asset.GetAsync<Shader>("Shaders/Generic");
				Material material = new Material(shader);

				materials[i] = material;
			}

			// Create meshes.
			foreach (var importMesh in importScene.Meshes)
			{
				// Interpret vertices.
				for (int j = 0; j < importMesh.VertexCount; j++)
				{
					positions.Add(new(importMesh.Vertices[j].X, importMesh.Vertices[j].Y, importMesh.Vertices[j].Z));
				}

				// Interpret normals.
				for (int j = 0; j < importMesh.VertexCount; j++)
				{
					normals.Add(new(importMesh.Normals[j].X, importMesh.Normals[j].Y, importMesh.Normals[j].Z));
				}

				// Interpret faces.
				var indices = importMesh.GetUnsignedIndices();
				for (int i = 0; i < indices.Length; i += 3)
				{
					Face face = new Face()
					{
						A = indexOffset + indices[i],
						B = indexOffset + indices[i + 1],
						C = indexOffset + indices[i + 2],
						Material = materials[importMesh.MaterialIndex]
					};

					faces.Add(face);
				}

				indexOffset = (uint)positions.Count;
			}

			// Create model.
			Model model = new Model();
			model.Parts = new[] { new ModelPart()
			{
				Faces = faces.ToArray(),
				Positions = positions.ToArray(),
				Normals = normals.ToArray(),
			}};

			return model;
		}
	}
}
