using System;
using Engine;
using Engine.Resources;
using Engine.Mathematics;
using Assimp;
using Assimp.Configs;
using AI = Assimp;
using ModelPart = Engine.Resources.ModelPart;
using Material = Engine.Resources.Material;
using System.Collections.Concurrent;
using Engine.Core;
using Mesh = Engine.Resources.Mesh;
using StbiSharp;

namespace Basic.Loaders
{
	public class GLTFLoader : AssetLoader<Model>
	{
		public string Path;

		[ThreadStatic]
		private static AssimpContext importContext = new();

		public GLTFLoader(string path)
		{
			Path = path;
		}

		public override async Task<Model> Load()
		{
			// Create import context if needed.
			if (importContext == null)
			{
				importContext = new AssimpContext();

				importContext.SetConfig(new AppScaleConfig(1));
				importContext.SetConfig(new FavorSpeedConfig(true));
			}

			// Load GLTF file from thread-local context.
			PostProcessSteps steps = PostProcessSteps.GenerateNormals | PostProcessSteps.GenerateUVCoords;
			//steps |= PostProcessSteps.CalculateTangentSpace;
			AI.Scene importScene = importContext.ImportFile(Path, steps);

			// Load embedded textures.
			/*Texture2D[] textures = new Texture2D[importScene.TextureCount];
			Parallel.For(0, importScene.TextureCount, (i) =>
			{
				using (StbiImage image = Stbi.LoadFromMemory(importScene.Textures[i].CompressedData, 4))
				{
					Texture2D texture = new Texture2D(image.Width, image.Height);

					Span<byte> imageData = null;
					unsafe
					{
						fixed (byte* dataPtr = image.Data)
						{
							imageData = new Span<byte>(dataPtr, image.Data.Length * sizeof(byte));
						}
					}
					
					texture.LoadData(imageData, TextureCompression.None);
					textures[i] = texture;
				}
			});*/

			// Create embedded materials.
			Material[] materials = new Material[importScene.MaterialCount];
			for (int i = 0; i < importScene.MaterialCount; i++)
			{
				Shader shader = await Asset.GetAsync<Shader>("USER:Shaders/PBR");
				Material material = new Material(shader);

				AI.Material importMaterial = importScene.Materials[i];

				// Assign textures.
				/*if (importMaterial.HasTextureDiffuse)
				{
					int textureIdx = int.Parse(importMaterial.TextureDiffuse.FilePath.Split('*')[1]);
					material.SetTexture("BaseColor", textures[textureIdx]);
				}
				if (importMaterial.HasTextureNormal)
				{
					int textureIdx = int.Parse(importMaterial.TextureNormal.FilePath.Split('*')[1]);
					material.SetTexture("Normal", textures[textureIdx]);
				}*/

				materials[i] = material;
			}

			// Create submeshes.
			ConcurrentBag<Mesh> meshes = new();
			Parallel.ForEach(importScene.Meshes, (importMesh, state) =>
			{
				Debug.Assert(importMesh.PrimitiveType == PrimitiveType.Triangle, "Engine does not support non-triangle geometry.");

				Vector3[] vertices = new Vector3[importMesh.VertexCount];
				Vector3[] normals = new Vector3[importMesh.VertexCount];
				Vector3[] uvs = new Vector3[importMesh.VertexCount];

				// Interpret vertices/normals.
				for (int i = 0; i < importMesh.VertexCount; i++)
				{
					vertices[i] = new(importMesh.Vertices[i].X, importMesh.Vertices[i].Y, importMesh.Vertices[i].Z);
					normals[i] = new(importMesh.Normals[i].X, importMesh.Normals[i].Y, importMesh.Normals[i].Z);
					uvs[i] = new(importMesh.TextureCoordinateChannels[0][i].X, importMesh.TextureCoordinateChannels[0][i].Y, importMesh.TextureCoordinateChannels[0][i].Z);
				}

				// Create submesh.
				Mesh submesh = new Mesh();
				submesh.SetMaterial(materials[importMesh.MaterialIndex]);

				// Note: merge these?
				submesh.SetVertices(vertices);
				//submesh.SetUVs(uvs);
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
