using System;
using NFM.GPU;
using NFM.Resources;

namespace NFM.Graphics;

class RenderMesh : IDisposable
{
	// Geometry buffers
	internal static GraphicsBuffer<uint> IndexBuffer = new(20000000 * 3); // Support 20m tris
	internal static GraphicsBuffer<Vertex> VertexBuffer = new(20000000); // Support 20m verts
	internal static GraphicsBuffer<Meshlet> MeshletBuffer = new(20000000); // Support 20m meshlets
	internal static GraphicsBuffer<GPUMesh> MeshBuffer = new(20000000 + 1); // Support 20m meshes

	static RenderMesh()
	{
		MeshBuffer.Name = "Mesh Buffer";
		MeshBuffer.Allocate(1, true); // First element is reserved to represent an invalid index.
	}

	// Geometry allocations
	internal BufferAllocation<uint> IndexHandle;
	internal BufferAllocation<Vertex> VertexHandle;
	internal BufferAllocation<Meshlet> MeshletHandle;
	internal BufferAllocation<GPUMesh> MeshHandle;

	public unsafe RenderMesh(Mesh source)
	{
		// Free existing allocations.
		VertexHandle?.Dispose();
		IndexHandle?.Dispose();
		MeshHandle?.Dispose();
		MeshletHandle?.Dispose();

		fixed (uint* indicesPtr = source.Indices)
		{
			// Build meshlet data
			MeshOptimizer.BuildMeshlets(source.Indices, source.Vertices.Length, out var meshletIndices, out var meshletVerts, out var meshlets);

			// Remap vertices to match meshlet output
			Vertex[] verts = new Vertex[meshletVerts.Length];
			for (int i = 0; i < meshletVerts.Length; i++)
			{
				verts[i] = source.Vertices[meshletVerts[i]];
			}

			// Cast indices byte->uint (TODO: unpack on the GPU)
			var resizedMeshletIndices = new uint[meshletIndices.Length];
			for (int i = 0; i < meshletIndices.Length; i++)
			{
				resizedMeshletIndices[i] = meshletIndices[i];
			}

			// Upload geometry data to GPU
			VertexHandle = VertexBuffer.Allocate(verts.Length);
			IndexHandle = IndexBuffer.Allocate(meshletIndices.Length);
			MeshletHandle = MeshletBuffer.Allocate(meshlets.Length);
			Renderer.DefaultCommandList.UploadBuffer(VertexHandle, verts);
			Renderer.DefaultCommandList.UploadBuffer(IndexHandle, resizedMeshletIndices);
			Renderer.DefaultCommandList.UploadBuffer(MeshletHandle, meshlets);
		}

		// Upload mesh info to GPU.
		MeshHandle = MeshBuffer.Allocate(1);
		Renderer.DefaultCommandList.UploadBuffer(MeshHandle, new GPUMesh()
		{
			MeshletCount = (uint)MeshletHandle.Size,
			MeshletOffset = (uint)MeshletHandle.Offset,
			IndexOffset = (uint)IndexHandle.Offset,
			VertexOffset = (uint)VertexHandle.Offset,
		});
	}

	public void Dispose()
	{
		IndexHandle?.Dispose();
		VertexHandle?.Dispose();
		MeshletHandle?.Dispose();
		MeshHandle?.Dispose();
	}
}

[StructLayout(LayoutKind.Sequential)]
internal struct GPUMesh
{
	public required uint VertexOffset; // Start of submesh in vertex buffer.
	public required uint IndexOffset; // Start of submesh in index buffer.
	public required uint MeshletOffset; // Start of submesh in meshlet buffer.
	public required uint MeshletCount;   // Number of meshlets used
}