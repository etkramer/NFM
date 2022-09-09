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
		private static AssimpContext aiContext = new();

		public GLTFLoader(string path)
		{
			Path = path;
		}

		public override async Task<Model> Load()
		{
			// Create import context if needed.
			if (aiContext == null)
			{
				aiContext = new AssimpContext();

				aiContext.SetConfig(new AppScaleConfig(1));
				aiContext.SetConfig(new FavorSpeedConfig(true));
			}

			// Load GLTF file from thread-local context.
			PostProcessSteps steps = PostProcessSteps.GenerateNormals | PostProcessSteps.GenerateUVCoords | PostProcessSteps.EmbedTextures | PostProcessSteps.FlipUVs;
			AI.Scene scene = aiContext.ImportFile(Path, steps);

			// Load embedded textures.
			Texture2D[] textures = new Texture2D[scene.TextureCount];
			Parallel.For(0, scene.TextureCount, (i) =>
			{
				using (StbiImage image = Stbi.LoadFromMemory(scene.Textures[i].CompressedData, 4))
				{
					Texture2D gameTexture = new Texture2D(image.Width, image.Height);

					Span<byte> imageData = null;
					unsafe
					{
						fixed (byte* dataPtr = image.Data)
						{
							imageData = new Span<byte>(dataPtr, image.Data.Length * sizeof(byte));
						}
					}
					
					gameTexture.LoadData(imageData, TextureCompression.None);
					gameTexture.GenerateMips();
					textures[i] = gameTexture;
				}
			});

			// Create embedded materials.
			Material[] gameMaterials = new Material[scene.MaterialCount];
			for (int i = 0; i < scene.MaterialCount; i++)
			{
				Shader shader = await Asset.GetAsync<Shader>("USER:Shaders/PBR.hlsl");
				Material gameMaterial = new Material(shader);

				AI.Material material = scene.Materials[i];

				material.GetMaterialTexture(TextureType.Diffuse, 0, out var color);
				material.GetMaterialTexture(TextureType.Normals, 0, out var normal);
				material.GetMaterialTexture(TextureType.Unknown, 0, out var orm);

				if (color.FilePath != null)
				{
					int textureIndex = int.Parse(color.FilePath.Split('*')[1]);
					gameMaterial.SetTexture("BaseColor", textures[textureIndex]);
				}

				if (normal.FilePath != null)
				{
					int textureIndex = int.Parse(normal.FilePath.Split('*')[1]);
					gameMaterial.SetTexture("Normal", textures[textureIndex]);
				}

				if (orm.FilePath != null)
				{
					int textureIndex = int.Parse(orm.FilePath.Split('*')[1]);
					gameMaterial.SetTexture("ORM", textures[textureIndex]);
				}

				gameMaterials[i] = gameMaterial;
			}

			// Create submeshes.
			ConcurrentBag<Mesh> gameMeshes = new();
			Parallel.ForEach(scene.Meshes, (mesh, state) =>
			{
				Debug.Assert(mesh.PrimitiveType == PrimitiveType.Triangle, "Engine does not support non-triangle geometry.");

				// Format vertices.
				Vertex[] vertices = new Vertex[mesh.VertexCount];
				for (int i = 0; i < mesh.VertexCount; i++)
				{
					vertices[i] = new Vertex()
					{
						Position = new Vector3(mesh.Vertices[i].X, mesh.Vertices[i].Y, mesh.Vertices[i].Z),
						Normal = new Vector3(mesh.Normals[i].X, mesh.Normals[i].Y, mesh.Normals[i].Z),
						UV0 = new Vector2(mesh.TextureCoordinateChannels[0][i].X, mesh.TextureCoordinateChannels[0][i].Y)
					};
				}

				// Create submesh.
				Mesh gameMesh = new Mesh();
				gameMesh.SetMaterial(gameMaterials[mesh.MaterialIndex]);
				gameMesh.SetVertices(vertices);
				gameMesh.SetIndices(mesh.GetUnsignedIndices());

				gameMeshes.Add(gameMesh);
			});

			// Create model.
			return new Model()
			{
				Parts = new ModelPart[]
				{
					new ModelPart(gameMeshes.ToArray())
				}
			};
		}
	}
}
