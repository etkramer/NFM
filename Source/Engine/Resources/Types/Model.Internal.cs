using System;
using System.Collections.Concurrent;
using System.Linq;
using Engine.GPU;
using MeshOptimizer;

namespace Engine.Resources
{
	// NOTES:
	// Model is just a collection of toggleable parts (equivalent to bodygroups) and a skeleton.
	// ModelPart owns complete vertex/index buffers.
	// Submesh is a material and a range within the index buffer.

	public partial class Model : Resource
	{
		#region Uploads
		public override void OnLoad()
		{
			foreach (ModelPart part in Parts)
			{
				part.BuildSubmeshes();
			}
		}
		#endregion
	}

	public partial class ModelPart
	{
		internal unsafe void BuildSubmeshes()
		{
			// Build submesh data.
			Parallel.ForEach(Submeshes, (submesh, state) =>
			{
				fixed (uint* indicesPtr = submesh.Triangles)
				{
					fixed (Vector3* vertsPtr = submesh.Vertices)
					{
						// Build meshlet data.
						MeshOperations.BuildMeshlets(submesh.Triangles.Length, indicesPtr, submesh.Vertices.Length, vertsPtr, sizeof(Vertex), out var prims, out var verts, out var meshlets);

						// Fetch final verts from meshlet output.
						Vertex[] vertsData = new Vertex[verts.Length];
						for (int i = 0; i < verts.Length; i++)
						{
							vertsData[i] = new Vertex()
							{
								Position = submesh.Vertices[verts[i]],
								Normal = submesh.Normals[verts[i]]
							};
						}

						// Upload geometry data to GPU.
						submesh.PrimHandle = Submesh.PrimBuffer.Upload(prims.Select(o => (uint)o).ToArray());
						submesh.VertHandle = Submesh.VertBuffer.Upload(vertsData);
						submesh.MeshletHandle = Submesh.MeshletBuffer.Upload(meshlets);
						submesh.MeshHandle = Submesh.MeshBuffer.Upload(new Mesh()
						{
							MeshletCount = (uint)submesh.MeshletHandle.ElementCount,
							MeshletOffset = (uint)submesh.MeshletHandle.ElementStart,
							PrimOffset = (uint)submesh.PrimHandle.ElementStart,
							VertOffset = (uint)submesh.VertHandle.ElementStart,
						});
					}
				}
			});
		}
	}

	public partial class Submesh
	{
		internal static GraphicsBuffer<uint> PrimBuffer = new(2000000);
		internal static GraphicsBuffer<Vertex> VertBuffer = new(2000000);
		internal static GraphicsBuffer<Meshlet> MeshletBuffer = new(2000000);
		internal static GraphicsBuffer<Mesh> MeshBuffer = new(100000);

		internal BufferHandle<uint> PrimHandle;
		internal BufferHandle<Vertex> VertHandle;
		internal BufferHandle<Meshlet> MeshletHandle;
		internal BufferHandle<Mesh> MeshHandle;
	}

	internal struct Mesh
	{
		public uint VertOffset; // Start of submesh in vertex buffer.
		public uint PrimOffset; // Start of submesh in primitive buffer.
		public uint MeshletOffset; // Start of submesh in meshlet buffer.
		public uint MeshletCount;   // Number of meshlets used
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct Vertex
	{
		public Vector3 Position;
		public Vector3 Normal;
	}
}