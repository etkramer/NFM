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
		public Vertex[] Vertices { get => vertices; set => SetVertices(value); }

		private uint[] indices;
		private Vertex[] vertices;

		public void Clear()
		{
			indices = null;
			vertices = null;

			areVerticesUploaded = false;
			areIndicesUploaded = false;

			PrimHandle?.Dispose();
			VertHandle?.Dispose();
			MeshletHandle?.Dispose();
			MeshHandle?.Dispose();
		}

		private uint[] vertMapping = null;
		private bool areIndicesUploaded = false;
		private bool areVerticesUploaded = false;

		public void SetIndices(uint[] value)
		{
			indices = value;

			if (vertices != null && !areVerticesUploaded)
			{
				UploadVertices();
				UploadIndices(); // Also builds meshlets, thus requiring vertex data.
			}
		}

		public void SetVertices(Vertex[] value)
		{
			vertices = value;

			UploadVertices();

			if (!areIndicesUploaded && indices != null)
			{
				UploadIndices();
			}
		}

		public void SetMaterial(Material value)
		{
			Material = value;
		}

		private void UploadIndices()
		{
			unsafe
			{
				fixed (uint* indicesPtr = indices)
				{
					fixed (Vertex* vertsPtr = vertices)
					{
						// Build meshlet data.
						MeshOperations.BuildMeshlets(indicesPtr, indices.Length, vertsPtr, vertices.Length, sizeof(Vertex), out var prims, out var verts, out var meshlets);
						vertMapping = verts;

						// Upload meshlet/index data to GPU.
						PrimHandle = PrimBuffer.Allocate(prims.Length);
						Renderer.DefaultCommandList.UploadBuffer(PrimHandle, prims.Select(o => (uint)o).ToArray());
						MeshletHandle = MeshletBuffer.Allocate(meshlets.Length);
						Renderer.DefaultCommandList.UploadBuffer(MeshletHandle, meshlets);
					}
				}
			}

			// Mark indices as uploaded.
			areIndicesUploaded = true;

			if (!areVerticesUploaded)
			{
				UploadVertices();
			}
			
			if (areVerticesUploaded)
			{
				TryUploadMesh();
			}
		}

		private void UploadVertices()
		{
			// We haven't generated meshlets yet, so we can't remap these vertices.
			// Should try to call this again in UploadIndices().
			if (!areIndicesUploaded)
			{
				return;
			}

			// Remap vertices to fit meshlets.
			Vertex[] remapped = RemapVerts();

			// Upload remapped verts to the vertex buffer.
			VertHandle?.Dispose();
			VertHandle = VertBuffer.Allocate(remapped.Length);
			Renderer.DefaultCommandList.UploadBuffer(VertHandle, remapped);

			// Mark vertices as uploaded.
			areVerticesUploaded = true;

			// See if we can upload the final mesh data yet.
			if (areIndicesUploaded)
			{
				TryUploadMesh();
			}
		}

		private void TryUploadMesh()
		{
			// Not ready to do the final upload yet.
			if (VertHandle == null || MeshletHandle == null || PrimHandle == null)
			{
				return;
			}

			MeshHandle?.Dispose();
			MeshHandle = MeshBuffer.Allocate(1);
			Renderer.DefaultCommandList.UploadBuffer(MeshHandle, new MeshData()
			{
				MeshletCount = (uint)MeshletHandle.Count,
				MeshletOffset = (uint)MeshletHandle.Start,
				PrimOffset = (uint)PrimHandle.Start,
				VertOffset = (uint)VertHandle.Start,
			});
		}

		private Vertex[] RemapVerts()
		{
			Vertex[] vertData = new Vertex[vertMapping.Length];
			for (int i = 0; i < vertMapping.Length; i++)
			{
				vertData[i] = vertices[vertMapping[i]];
			}

			return vertData;
		}
	}
}