﻿using System;
using System.Runtime.InteropServices;
using Engine;
using Engine.Resources;
using Engine.Mathematics;
using Asset = Engine.Resources.Asset;
using ModelPart = Engine.Resources.ModelPart;
using Material = Engine.Resources.Material;
using Mesh = Engine.Resources.Mesh;
using SharpGLTF.Schema2;
using SharpGLTF.Validation;
using StbiSharp;

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
			Parallel.ForEach(model.LogicalTextures, (texture, ct) =>
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
				Parallel.ForEach(mesh.Primitives, (primitive) =>
				{
					// Find node and read transform.
					var node = model.LogicalNodes.FirstOrDefault(o => o.Mesh == mesh);
					var worldMatrix = ((Matrix4)node.GetWorldMatrix(null, 0)).Transpose();

					// Transform to Z-up.
					worldMatrix = Matrix4.CreateRotation(new(90, 0, 0)) * worldMatrix;

					// Grab vertex spans from GLTF.
					var positions = primitive.GetVertexAccessor("POSITION").AsSpan<Vector3>();
					var normals = primitive.GetVertexAccessor("NORMAL").AsSpan<Vector3>();
					var uvs = primitive.GetVertexAccessor("TEXCOORD_0").AsSpan<Vector2>();

					// Read vertex data from accessor streams.
					Vertex[] vertices = new Vertex[positions.Length];
					for (int i = 0; i < positions.Length; i++)
					{
						vertices[i] = new Vertex()
						{
							Position = (new Vector4(positions[i].X, positions[i].Y, positions[i].Z, 1) * worldMatrix).Xyz,
							Normal = (new Vector4(normals[i].X, normals[i].Y, normals[i].Z, 1) * worldMatrix).Xyz,
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

	public static class GLTFHelpers
	{
		public static unsafe Span<T> AsSpan<T>(this Accessor accessor) where T : unmanaged
		{
			return MemoryMarshal.Cast<byte, T>(accessor.SourceBufferView.Content);
		}
	}
}
