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
using Engine.Common;
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
			PostProcessSteps steps = PostProcessSteps.GenerateNormals | PostProcessSteps.GenerateUVCoords | PostProcessSteps.EmbedTextures;
			AI.Scene importScene = importContext.ImportFile(Path, steps);

			// Load embedded textures.
			Texture2D[] textures = new Texture2D[importScene.TextureCount];
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
					texture.GenerateMips();
					textures[i] = texture;
				}
			});

			// Create embedded materials.
			Material[] materials = new Material[importScene.MaterialCount];
			for (int i = 0; i < importScene.MaterialCount; i++)
			{
				Shader shader = await Asset.GetAsync<Shader>("USER:Shaders/PBR.hlsl");
				Material material = new Material(shader);

				AI.Material importMaterial = importScene.Materials[i];

				importMaterial.GetMaterialTexture(TextureType.Diffuse, 0, out var color);
				importMaterial.GetMaterialTexture(TextureType.Normals, 0, out var normal);
				importMaterial.GetMaterialTexture(TextureType.Unknown, 0, out var orm);

				if (color.FilePath != null)
				{
					int textureIndex = int.Parse(color.FilePath.Split('*')[1]);
					material.SetTexture("BaseColor", textures[textureIndex]);
				}

				if (normal.FilePath != null)
				{
					int textureIndex = int.Parse(normal.FilePath.Split('*')[1]);
					material.SetTexture("Normal", textures[textureIndex]);
				}

				if (orm.FilePath != null)
				{
					int textureIndex = int.Parse(orm.FilePath.Split('*')[1]);
					material.SetTexture("ORM", textures[textureIndex]);
				}

				materials[i] = material;
			}

			// Create submeshes.
			ConcurrentBag<Mesh> meshes = new();
			Parallel.ForEach(importScene.Meshes, (importMesh, state) =>
			{
				Debug.Assert(importMesh.PrimitiveType == PrimitiveType.Triangle, "Engine does not support non-triangle geometry.");

				// Format vertices.
				Vertex[] vertices = new Vertex[importMesh.VertexCount];
				for (int i = 0; i < importMesh.VertexCount; i++)
				{
					vertices[i] = new Vertex()
					{
						Position = new Vector3(importMesh.Vertices[i].X, importMesh.Vertices[i].Y, importMesh.Vertices[i].Z),
						Normal = new Vector3(importMesh.Normals[i].X, importMesh.Normals[i].Y, importMesh.Normals[i].Z),
						UV0 = new Vector2(importMesh.TextureCoordinateChannels[0][i].X, importMesh.TextureCoordinateChannels[0][i].Y)
					};
				}

				// Create submesh.
				Mesh submesh = new Mesh();
				submesh.SetMaterial(materials[importMesh.MaterialIndex]);
				submesh.SetVertices(vertices);
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
