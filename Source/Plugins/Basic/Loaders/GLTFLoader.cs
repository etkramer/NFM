using System;
using Engine;
using Engine.Resources;
using Engine.Mathematics;
using ModelPart = Engine.Resources.ModelPart;
using Material = Engine.Resources.Material;
using System.Collections.Concurrent;
using Engine.Common;
using Mesh = Engine.Resources.Mesh;
using StbiSharp;
using SharpGLTF.Schema2;
using Asset = Engine.Resources.Asset;
using SharpGLTF.Validation;

namespace Basic.Loaders
{
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
			ModelRoot model = ModelRoot.Load(Path, new ReadSettings()
			{
				Validation = ValidationMode.Skip
			});

			// Load textures from GLTF
			Texture2D[] gameTextures = new Texture2D[model.LogicalTextures.Count];
			await Parallel.ForEachAsync(model.LogicalTextures, async (texture, ct) =>
			{
				using (StbiImage image = Stbi.LoadFromMemory(texture.PrimaryImage.Content.Content.Span, 4))
				{
					Texture2D gameTexture = new Texture2D(image.Width, image.Height);
					
					gameTexture.LoadData(ToReadWriteSpan(image.Data), TextureCompression.None);
					gameTexture.GenerateMips();
					gameTextures[texture.LogicalIndex] = gameTexture;
				}
			});

			// Load materials from GLTF
			Material[] gameMaterials = new Material[model.LogicalMaterials.Count];
			for (int i = 0; i < model.LogicalMaterials.Count; i++)
			{
				var material = model.LogicalMaterials[i];

				// Determine shader and create material.
				Shader shader = material.Alpha switch
				{
					AlphaMode.MASK => await Asset.Load<Shader>("USER:Shaders/Transparent.hlsl"),
					AlphaMode.BLEND => await Asset.Load<Shader>("USER:Shaders/Transparent.hlsl"),
					_ => await Asset.Load<Shader>("USER:Shaders/Opaque.hlsl")
				};
				Material gameMaterial = new Material(shader);

				// Loop over material channels
				foreach (var channel in material.Channels)
				{
					if (channel.Key == "BaseColor" && channel.Texture != null)
					{
						gameMaterial.SetTexture("BaseColor", gameTextures[channel.Texture.LogicalIndex]);
					}
					else if (channel.Key == "Normal" && channel.Texture != null)
					{
						gameMaterial.SetTexture("Normal", gameTextures[channel.Texture.LogicalIndex]);
					}
					else if (channel.Key == "MetallicRoughness" && channel.Texture != null)
					{
						gameMaterial.SetTexture("ORM", gameTextures[channel.Texture.LogicalIndex]);
					}
				}

				gameMaterials[i] = gameMaterial;
			}

			// Loop through GLTF "meshes" (equivalent to ModelParts)
			ModelPart[] gameParts = new ModelPart[model.LogicalMeshes.Count];
			foreach (var mesh in model.LogicalMeshes)
			{
				// Loop through GLTF "primitives" (equivalent to Meshes)
				Mesh[] gameMeshes = new Mesh[mesh.Primitives.Count];
				await Parallel.ForEachAsync(mesh.Primitives, async (primitive, ct) =>
				{
					// Find node and read transform.
					var node = model.LogicalNodes.FirstOrDefault(o => o.Mesh == mesh);
					var worldMatrix = ((Matrix4)node.GetWorldMatrix(null, 0)).Transpose();

					// Grab vertex accessors from GLTF.
					var posAccessor = primitive.GetVertexAccessor("POSITION");
					var normAccessor = primitive.GetVertexAccessor("NORMAL");
					var uvAccessor = primitive.GetVertexAccessor("TEXCOORD_0");

					// Create arrays from accessors.
					var positions = posAccessor.AsVector3Array();
					var normals = normAccessor.AsVector3Array();
					var uvs = uvAccessor.AsVector2Array();

					// Read vertex data from accessor streams.
					Vertex[] vertices = new Vertex[positions.Count];
					for (int i = 0; i < positions.Count; i++)
					{
						vertices[i] = new Vertex()
						{
							Position = (new Vector4(positions[i].X, positions[i].Y, positions[i].Z, 1) * worldMatrix).Xyz,
							Normal = new Vector3(normals[i].X, normals[i].Y, normals[i].Z),
							UV0 = new Vector2(uvs[i].X, uvs[i].Y)
						};
					}

					// Create mesh and add to collection.
					Mesh gameMesh = new Mesh();
					gameMesh.SetMaterial(gameMaterials[primitive.Material.LogicalIndex]);
					gameMesh.SetVertices(vertices);
					gameMesh.SetIndices(primitive.GetIndices().ToArray());
					gameMesh.Commit();

					gameMeshes[primitive.LogicalIndex] = gameMesh;
				});

				// Create ModelParts from GLTF "meshes"
				gameParts[mesh.LogicalIndex] = new ModelPart(gameMeshes);
			}

			return new Model(gameParts);
		}

		private unsafe Span<T> ToReadWriteSpan<T>(ReadOnlySpan<T> source) where T : unmanaged
		{
			fixed (T* dataPtr = source)
			{
				return new Span<T>(dataPtr, source.Length);
			}
		}
	}
}
