using System;
using System.Linq;
using Engine.GPU;
using Engine.Rendering;
using MeshOptimizer;

namespace Engine.Resources
{
	/// <summary>
	/// A 3D model, composed of one or multiple parts and optionally a skeleton.
	/// </summary>
	public partial class Model : Resource
	{
		public ModelPart[] Parts { get; set; }
	}

	/// <summary>
	/// A group of one or multiple meshes. Can be toggled on/off within the editor, comparable to a bodygroup in SFM.
	/// </summary>
	public partial class ModelPart
	{
		public Mesh[] Meshes { get; set; }
	}

	/// <summary>
	/// A part of a model that contains geometry - each model must have at least one of these per unique material.
	/// </summary>
	public partial class Mesh
	{
		public Material Material { get; set; }

		public uint[] Indices { get => indices; set => SetIndices(value); }
		public Vector3[] Vertices { get => vertices; set => SetVertices(value); }
		public Vector3[] Normals { get => normals; set => SetNormals(value); }

		private uint[] indices;
		private Vector3[] vertices;
		private Vector3[] normals;

		public void Clear()
		{
			indices = null;
			vertices = null;
			normals = null;

			PrimHandle?.Dispose();
			VertHandle?.Dispose();
			MeshletHandle?.Dispose();
			MeshHandle?.Dispose();
		}

		private uint[] vertMapping = null;

		public void SetIndices(uint[] value)
		{
			Debug.Assert(indices == null, "Cannot set mesh indices multiple times between calls to Mesh.Clear()");
			indices = value;

			foreach (uint index in value)
			{
				Debug.Assert(index < vertices.Length, "Supplied mesh indices are out of bounds.");
			}

			unsafe
			{
				fixed (uint* indicesPtr = indices)
				{
					fixed (Vector3* vertsPtr = vertices)
					{
						// Build meshlet data.
						MeshOperations.BuildMeshlets(indicesPtr, indices.Length, vertsPtr, vertices.Length, sizeof(Vector3), out var prims, out var verts, out var meshlets);

						vertMapping = verts;
						if (vertices != null && normals != null)
						{
							// Upload meshlet-remapped verts.
							var remapped = RemapVerts();
							VertHandle = VertBuffer.Allocate(remapped.Length);
							Renderer.DefaultCommandList.UploadBuffer(VertHandle, remapped);
						}

						// Upload meshlet/index data to GPU.
						PrimHandle = PrimBuffer.Allocate(prims.Length);
						Renderer.DefaultCommandList.UploadBuffer(PrimHandle, prims.Select(o => (uint)o).ToArray());
						MeshletHandle = MeshletBuffer.Allocate(meshlets.Length);
						Renderer.DefaultCommandList.UploadBuffer(MeshletHandle, meshlets);

						TryUploadMesh();
					}
				}
			}
		}

		public void SetVertices(Vector3[] value)
		{
			Debug.Assert(normals == null || normals?.Length == value?.Length, "Vertex/normal count must match!");
			vertices = value;

			// Upload meshlet-remapped verts.
			if (vertMapping != null && normals != null)
			{
				var remapped = RemapVerts();
				VertHandle = VertBuffer.Allocate(remapped.Length);
				Renderer.DefaultCommandList.UploadBuffer(VertHandle, remapped);
			}

			TryUploadMesh();
		}

		public void SetNormals(Vector3[] value)
		{
			Debug.Assert(vertices == null || vertices?.Length == value?.Length, "Vertex/normal count must match!");
			normals = value;

			// Upload meshlet-remapped verts.
			if (vertMapping != null && vertices != null)
			{
				var remapped = RemapVerts();
				VertHandle = VertBuffer.Allocate(remapped.Length);
				Renderer.DefaultCommandList.UploadBuffer(VertHandle, remapped);
			}

			TryUploadMesh();
		}

		public void SetMaterial(Material value)
		{
			Material = value;
		}

		private void TryUploadMesh()
		{
			// Not ready to do the final upload yet.
			if (MeshHandle != null || VertHandle == null || MeshletHandle == null || PrimHandle == null)
			{
				return;
			}

			MeshHandle = MeshBuffer.Allocate(1);
			Renderer.DefaultCommandList.UploadBuffer(MeshHandle, new MeshData()
			{
				MeshletCount = (uint)MeshletHandle.Count,
				MeshletOffset = (uint)MeshletHandle.Start,
				PrimOffset = (uint)PrimHandle.Start,
				VertOffset = (uint)VertHandle.Start,
			});
		}

		private VertexData[] RemapVerts()
		{
			VertexData[] vertData = new VertexData[vertMapping.Length];
			for (int i = 0; i < vertMapping.Length; i++)
			{
				vertData[i] = new VertexData()
				{
					Position = vertices[vertMapping[i]],
					Normal = normals[vertMapping[i]]
				};
			}

			return vertData;
		}
	}
}