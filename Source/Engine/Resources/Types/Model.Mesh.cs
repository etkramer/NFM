using System;
using Engine.GPU;
using Engine.Rendering;
using MeshOptimizer;

namespace Engine.Resources
{
	/// <summary>
	/// A part of a model that contains geometry - each model must have at least one of these per unique material.
	/// </summary>
	public partial class Mesh : IDisposable
	{
		// Geometry buffers
		internal static GraphicsBuffer<uint> PrimBuffer = new(2000000);
		internal static GraphicsBuffer<Vertex> VertBuffer = new(2000000);
		internal static GraphicsBuffer<Meshlet> MeshletBuffer = new(2000000);
		internal static GraphicsBuffer<GPUMesh> MeshBuffer = new(1000000);

		// Geometry allocations
		internal BufferAllocation<uint> PrimHandle;
		internal BufferAllocation<Vertex> VertHandle;
		internal BufferAllocation<Meshlet> MeshletHandle;
		internal BufferAllocation<GPUMesh> MeshHandle;

		public bool IsCommitted { get; private set; } = false;

		public uint[] Indices { get; set; }
		public Vertex[] Vertices { get; set; }
		public Material Material { get; set; }

		public void SetIndices(uint[] value)
		{
			Indices = value;
		}

		public void SetVertices(Vertex[] value)
		{
			Vertices = value;
		}

		public void SetMaterial(Material value)
		{
			Material = value;
		}

		public void Commit()
		{
			unsafe
			{
				fixed (uint* indicesPtr = Indices)
				{
					fixed (Vertex* vertsPtr = Vertices)
					{
						// Build meshlet data.
						MeshOperations.BuildMeshlets(indicesPtr, Indices.Length, vertsPtr, Vertices.Length, sizeof(Vertex), out var prims, out var verts, out var meshlets);

						// Remap vertices to match meshlet output.
						Vertex[] remappedVerts = new Vertex[verts.Length];
						for (int i = 0; i < verts.Length; i++)
						{
							remappedVerts[i] = Vertices[verts[i]];
						}

						// Upload vertex data to GPU.
						VertHandle = VertBuffer.Allocate(remappedVerts.Length);
						Renderer.DefaultCommandList.UploadBuffer(VertHandle, remappedVerts);

						// Upload meshlet/index data to GPU.
						PrimHandle = PrimBuffer.Allocate(prims.Length);
						Renderer.DefaultCommandList.UploadBuffer(PrimHandle, prims.Select(o => (uint)o).ToArray());
						MeshletHandle = MeshletBuffer.Allocate(meshlets.Length);
						Renderer.DefaultCommandList.UploadBuffer(MeshletHandle, meshlets);

						// Upload mesh info to GPU.
						MeshHandle = MeshBuffer.Allocate(1);
						Renderer.DefaultCommandList.UploadBuffer(MeshHandle, new GPUMesh()
						{
							MeshletCount = (uint)MeshletHandle.Count,
							MeshletOffset = (uint)MeshletHandle.Start,
							PrimOffset = (uint)PrimHandle.Start,
							VertOffset = (uint)VertHandle.Start,
						});
					}
				}
			}

			// Mark mesh as committed.
			IsCommitted = true;
		}

		public void Dispose()
		{
			PrimHandle?.Dispose();
			VertHandle?.Dispose();
			MeshletHandle?.Dispose();
			MeshHandle?.Dispose();
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct GPUMesh
	{
		public uint VertOffset; // Start of submesh in vertex buffer.
		public uint PrimOffset; // Start of submesh in primitive buffer.
		public uint MeshletOffset; // Start of submesh in meshlet buffer.
		public uint MeshletCount;   // Number of meshlets used
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct Vertex
	{
		public Vector3 Position;
		public Vector3 Normal;
		public Vector2 UV0;
	}
}