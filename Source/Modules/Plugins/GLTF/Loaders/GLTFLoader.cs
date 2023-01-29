using System;
using System.Collections;
using System.Runtime.InteropServices;
using NFM;
using NFM.Resources;
using NFM.Mathematics;
using Asset = NFM.Resources.Asset;
using Material = NFM.Resources.Material;
using Mesh = NFM.Resources.Mesh;
using SharpGLTF.Schema2;
using SharpGLTF.Validation;
using StbiSharp;
using NFM.Common;
using SharpGLTF.IO;

namespace GLTF.Loaders
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
			ModelRoot sourceModel = ModelRoot.Load(Path, ValidationMode.Skip);

			// Load textures from GLTF
			Texture2D[] textures = new Texture2D[sourceModel.LogicalTextures.Count];
			Parallel.ForEach(sourceModel.LogicalTextures, (sourceTexture, ct) =>
			{
				using (StbiImage sourceImage = Stbi.LoadFromMemory(sourceTexture.PrimaryImage.Content.Content.Span, 4))
				{
					Texture2D texture = new Texture2D(sourceImage.Width, sourceImage.Height, TextureFormat.RGBA8, 4);
					texture.SetPixelData(ToReadWriteSpan(sourceImage.Data), 0, true);

					textures[sourceTexture.LogicalIndex] = texture;
				}
			});

			// Load materials from GLTF
			Material[] materials = new Material[sourceModel.LogicalMaterials.Count];
			for (int i = 0; i < sourceModel.LogicalMaterials.Count; i++)
			{
				var sourceMaterial = sourceModel.LogicalMaterials[i];

				// Check if this material uses KHR_materials_transmission
				bool useTransmission = sourceMaterial.FindChannel("Transmission") != null;

				// Determine shader
				Shader shader = sourceMaterial.Alpha switch
				{
					AlphaMode.MASK => await Asset.LoadAsync<Shader>("USER:/Shaders/Transparent.hlsl"),
					AlphaMode.BLEND => await Asset.LoadAsync<Shader>("USER:/Shaders/Transparent.hlsl"),
					_ => await Asset.LoadAsync<Shader>("USER:/Shaders/Opaque.hlsl")
				};

				// Create material from channels
				Material material = new Material(shader);
				foreach (var channel in sourceMaterial.Channels)
				{
					if (channel.Key == "BaseColor" && channel.Texture != null)
					{
						material.SetTexture("BaseColor", textures[channel.Texture.LogicalIndex]);
					}
					else if (channel.Key == "Normal" && channel.Texture != null)
					{
						material.SetTexture("Normal", textures[channel.Texture.LogicalIndex]);
					}
					else if (channel.Key == "MetallicRoughness" && channel.Texture != null)
					{
						material.SetTexture("ORM", textures[channel.Texture.LogicalIndex]);
					}
				}

				materials[i] = material;
			}
			
			Debug.Assert(sourceModel.LogicalSkins.Count <= 1, "GLTF models with multiple skeletons are not supported");

			// Create model for NFM
			Model model = new Model();

			// Build model parts (one per GLTF mesh)
			foreach (var node in sourceModel.LogicalNodes.Where(o => o.Mesh != null))
			{
				var sourceMesh = node.Mesh;

				Parallel.ForEach(sourceMesh.Primitives, (primitive) =>
				{
					// Get node transform as Z-up
					var worldMatrix = (Matrix4)node.WorldMatrix;
					worldMatrix *= Matrix4.CreateRotation(new(90, 0, 0));

					// Build vertices for the base mesh
					var baseVertices = BuildVertices(primitive.VertexAccessors, worldMatrix);

					// Create mesh and add to collection
					var mesh = new Mesh(sourceMesh.Name ?? "unnamed");
					mesh.SetVertices(baseVertices);
					mesh.SetIndices(primitive.GetIndices().ToArray());
					mesh.SetMaterial(materials[primitive.Material.LogicalIndex]);

					// Add to new mesh (body) group
					model.AddMeshGroup(mesh.Name, new Mesh[] { mesh, null }, mesh);
				});
			}

			return model;
		}

		private Vertex[] BuildVertices(IReadOnlyDictionary<string, Accessor> accessors, Matrix4 transform)
		{
			var positions = accessors.GetValueOrDefault("POSITION").AsSpan<Vector3>();
			var normals = accessors.GetValueOrDefault("NORMAL").AsSpan<Vector3>();
			var uv0 = accessors.GetValueOrDefault("TEXCOORD_0").AsSpan<Vector2>();
			var uv1 = accessors.GetValueOrDefault("TEXCOORD_1").AsSpan<Vector2>();

			Vertex[] result = new Vertex[positions.Length];
			for (int i = 0; i < result.Length; i++)
			{
				Vertex vertex = new Vertex();
				vertex.Position = (new Vector4(positions[i].X, positions[i].Y, positions[i].Z, 1) * transform).Xyz;

				if (normals != null)
				{
					vertex.Normal = (new Vector4(normals[i].X, normals[i].Y, normals[i].Z, 1) * transform).Xyz;
				}
				if (uv0 != null)
				{
					vertex.UV0 = new Vector2(uv0[i].X, uv0[i].Y);
				}
				if (uv1 != null)
				{
					vertex.UV1 = new Vector2(uv1[i].X, uv1[i].Y);
				}

				result[i] = vertex;
			}

			return result;
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
			if (accessor == null)
			{
				return null;
			}

			var slice = accessor.SourceBufferView.Content.Slice(accessor.ByteOffset, accessor.ByteLength);
			return MemoryMarshal.Cast<byte, T>(slice);
		}

		public static bool TryGetNode(this JsonContent source, string name, out JsonContent result)
		{
			try
			{
				result = source.GetNode(name);
				return true;
			}
			catch
			{
				result = default;
				return false;
			}
		}
	}
}
