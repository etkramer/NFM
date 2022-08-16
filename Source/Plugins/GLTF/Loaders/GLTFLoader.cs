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

			int indexOffset = 0;
			List<Vector3> Positions = new();
			List<Vector3> Normals = new();
			List<Face> Faces = new();

			foreach (var importMesh in importScene.Meshes)
			{
				// Interpret vertices.
				for (int j = 0; j < importMesh.VertexCount; j++)
				{
					Positions.Add(new(importMesh.Vertices[j].X, importMesh.Vertices[j].Y, importMesh.Vertices[j].Z));
				}

				// Interpret normals.
				for (int j = 0; j < importMesh.VertexCount; j++)
				{
					Normals.Add(new(importMesh.Normals[j].X, importMesh.Normals[j].Y, importMesh.Normals[j].Z));
				}

				// Interpret material. NOTE: There's no need to make separate assets for embedded content.
				var importMaterial = importScene.Materials[importMesh.MaterialIndex];
				Shader shader = await Asset.GetAsync<Shader>("USER:Shaders/Generic");
				Material material = new Material(shader);

				// Interpret faces (indices).
				var indices = importMesh.GetIndices();
				var faces = new Face[indices.Length / 3];
				for (int j = 0; j < faces.Length; j++)
				{
					Face face = new Face();
					face.A = (uint)indexOffset + (uint)indices[(j * 3) + 0];
					face.B = (uint)indexOffset + (uint)indices[(j * 3) + 1];
					face.C = (uint)indexOffset + (uint)indices[(j * 3) + 2];
					face.Material = material;

					faces[j] = face;
				}

				Faces.AddRange(faces);
				indexOffset = Positions.Count;
			}

			// Create model.
			Model model = new Model();
			model.Parts = new[] { new ModelPart()
			{
				Faces = Faces.ToArray(),
				Positions = Positions.ToArray(),
				Normals = Normals.ToArray(),
			}};

			return model;
		}
	}
}
