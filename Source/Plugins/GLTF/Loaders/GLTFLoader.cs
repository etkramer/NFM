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
	public class GLTFLoader : AssetLoader<Model>
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
			Parallel.ForEach(model.LogicalTextures, (texture) =>
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
					AlphaMode.MASK => await Asset.GetAsync<Shader>("USER:Shaders/Transparent.hlsl"),
					AlphaMode.BLEND => await Asset.GetAsync<Shader>("USER:Shaders/Transparent.hlsl"),
					_ => await Asset.GetAsync<Shader>("USER:Shaders/Opaque.hlsl")
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
						gameMaterial.SetTexture("MetallicRoughness", gameTextures[channel.Texture.LogicalIndex]);
					}
				}

				gameMaterials[i] = gameMaterial;
			}

			// I've only ever observed a GLTF file having one of these, but I assume there could be more.
			Debug.Assert(model.LogicalMeshes.Count <= 1, "Not supported for the time being, because I don't have any examples of this in the wild.");

			// Load meshes from GLTF.
			Mesh[] gameMeshes = new Mesh[model.LogicalMeshes[0].Primitives.Count];
			Parallel.ForEach(model.LogicalMeshes[0].Primitives, (primitive) =>
			{
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
						Position = new Vector3(positions[i].X, positions[i].Y, positions[i].Z),
						Normal = new Vector3(normals[i].X, normals[i].Y, normals[i].Z),
						UV0 = new Vector2(uvs[i].X, uvs[i].Y)
					};
				}

				// Create mesh.
				Mesh gameMesh = new Mesh();
				gameMesh.SetMaterial(gameMaterials[primitive.Material.LogicalIndex]);
				gameMesh.SetVertices(vertices);
				gameMesh.SetIndices(primitive.GetIndices().ToArray());
				gameMeshes[primitive.LogicalIndex] = gameMesh;
			});

			return new Model(gameMeshes);
		}

		private Span<T> ToReadWriteSpan<T>(ReadOnlySpan<T> source) where T : unmanaged
		{
			unsafe
			{
				fixed (T* dataPtr = source)
				{
					return new Span<T>(dataPtr, source.Length);
				}
			}
		}
	}
}
