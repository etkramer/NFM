using System;
using System.Linq;
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

			PrimHandle?.Free();
			VertHandle?.Free();
			MeshletHandle?.Free();
			MeshHandle?.Free();
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
						MeshOperations.BuildMeshlets(indices.Length, indicesPtr, vertices.Length, vertsPtr, sizeof(VertexData), out var prims, out var verts, out var meshlets);

						vertMapping = verts;
						if (vertices != null && normals != null)
						{
							// Upload meshlet-remapped verts.
							VertHandle = VertBuffer.Upload(RemapVerts());
						}

						// Upload meshlet/index data to GPU.
						PrimHandle = PrimBuffer.Upload(prims.Select(o => (uint)o).ToArray());
						MeshletHandle = MeshletBuffer.Upload(meshlets);

						// Source index/vertex count is consistent.
						// Output index/vertex count is consistent.
						// Output meshlets count is not only out of order (fine), but inconsistent (bad)???
						//Debug.Log(meshlets.Length);

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
				VertHandle = VertBuffer.Upload(RemapVerts());
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
				VertHandle = VertBuffer.Upload(RemapVerts());
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

			MeshHandle = MeshBuffer.Upload(new MeshData()
			{
				MeshletCount = (uint)MeshletHandle.ElementCount,
				MeshletOffset = (uint)MeshletHandle.ElementStart,
				PrimOffset = (uint)PrimHandle.ElementStart,
				VertOffset = (uint)VertHandle.ElementStart,
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