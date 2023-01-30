using System;
using NFM.GPU;
using NFM.Resources;

namespace NFM.Graphics;

class RenderMesh : IDisposable
{
	// Geometry buffers
	internal static GraphicsBuffer<uint> IndexBuffer = new(20000000 * 3); // Support 20m tris
	internal static GraphicsBuffer<Vertex> VertexBuffer = new(20000000); // Support 20m verts
	internal static GraphicsBuffer<GPUMesh> MeshBuffer = new(20000000 + 1); // Support 20m meshes

	static RenderMesh()
	{
		MeshBuffer.Name = "Mesh Buffer";
		MeshBuffer.Allocate(1, true); // First element is reserved to represent an invalid index.
	}

	// Geometry allocations
	internal BufferAllocation<uint> IndexHandle;
	internal BufferAllocation<Vertex> VertexHandle;
	internal BufferAllocation<GPUMesh> MeshHandle;

	public unsafe RenderMesh(Mesh source)
	{
		// Free existing allocations.
		VertexHandle?.Dispose();
		IndexHandle?.Dispose();
		MeshHandle?.Dispose();

		fixed (uint* indicesPtr = source.Indices)
		{
			// Upload geometry data to GPU
			VertexHandle = VertexBuffer.Allocate(source.Vertices.Length);
			IndexHandle = IndexBuffer.Allocate(source.Indices.Length);
			Renderer.DefaultCommandList.UploadBuffer(VertexHandle, source.Vertices);
			Renderer.DefaultCommandList.UploadBuffer(IndexHandle, source.Indices);
		}

		// Upload mesh info to GPU.
		MeshHandle = MeshBuffer.Allocate(1);
		Renderer.DefaultCommandList.UploadBuffer(MeshHandle, new GPUMesh()
		{
			VertexOffset = (uint)VertexHandle.Offset,
			IndexOffset = (uint)IndexHandle.Offset,
			IndexCount = (uint)source.Indices.Length,
		});
	}

	public void Dispose()
	{
		IndexHandle?.Dispose();
		VertexHandle?.Dispose();
		MeshHandle?.Dispose();
	}
}

[StructLayout(LayoutKind.Sequential)]
internal struct GPUMesh
{
	public required uint VertexOffset; // Start of vertices in vertex buffer
	public required uint IndexOffset; // Start of indices in index buffer
	public required uint IndexCount; // Number of indices in index buffer
}