using NFM.GPU;
using NFM.Resources;

namespace NFM.Graphics;

class RenderMesh : IDisposable
{
	// Geometry buffers
	internal static TypedBuffer<uint> IndexBuffer = new(20000000 * 3); // Support 20m tris
	internal static TypedBuffer<Vertex> VertexBuffer = new(20000000); // Support 20m verts
	internal static TypedBuffer<MeshData> MeshBuffer = new(20000000 + 1); // Support 20m meshes
	internal static TypedBuffer<VertexWeights> WeightBuffer = new(4000000); // Support 4m skinned verts

	static RenderMesh()
	{
		MeshBuffer.Name = "Mesh Buffer";
		WeightBuffer.Name = "Weight Buffer";
		MeshBuffer.Allocate(1, true); // First element is reserved to represent an invalid index.
	}

	/// <summary>
	/// Meshes whose acceleration structure hasn't been built yet, drained by the BVH pass.
	/// </summary>
	internal static HashSet<RenderMesh> PendingBuilds { get; } = [];

	// Geometry allocations
	internal BufferAllocation<uint> IndexHandle;
	internal BufferAllocation<Vertex> VertexHandle;
	internal BufferAllocation<MeshData> MeshHandle;
	internal BufferAllocation<VertexWeights>? WeightHandle;

	/// <summary>
	/// Traced against by every instance of this mesh that isn't deformed.
	/// </summary>
	internal BottomLevelAS BLAS;

	public unsafe RenderMesh(Mesh source)
	{
        Guard.NotNull(source.Vertices);
        Guard.NotNull(source.Indices);

		// Upload geometry data to GPU
		VertexHandle = VertexBuffer.Allocate(source.Vertices.Length);
		IndexHandle = IndexBuffer.Allocate(source.Indices.Length);
		Renderer.DefaultCommandList.UploadBuffer(VertexHandle, source.Vertices);
		Renderer.DefaultCommandList.UploadBuffer(IndexHandle, source.Indices);

		if (source.Weights is not null)
		{
			WeightHandle = WeightBuffer.Allocate(source.Weights.Length);
			Renderer.DefaultCommandList.UploadBuffer(WeightHandle, source.Weights);
		}

		BLAS = new BottomLevelAS(VertexBuffer, VertexHandle.Offset, VertexHandle.Size, IndexBuffer, IndexHandle.Offset, IndexHandle.Size);
		PendingBuilds.Add(this);

		// Upload mesh info to GPU.
		MeshHandle = MeshBuffer.Allocate(1);
		Renderer.DefaultCommandList.UploadBuffer(MeshHandle, new MeshData()
		{
			VertexOffset = (uint)VertexHandle.Offset,
			IndexOffset = (uint)IndexHandle.Offset,
			IndexCount = (uint)source.Indices.Length,
		});
	}

	public void Dispose()
	{
		PendingBuilds.Remove(this);

		IndexHandle?.Dispose();
		VertexHandle?.Dispose();
		MeshHandle?.Dispose();
		WeightHandle?.Dispose();
		BLAS.Dispose();
	}
}

/// <summary>
/// One instance's deformed copy of a skinned mesh, and the structure traced against it. Deformed
/// geometry can't share the mesh's structure, so each instance carries its own.
/// </summary>
class RenderSkin : IDisposable
{
	public required BufferAllocation<Vertex> Vertices { get; init; }
	public required BottomLevelAS BLAS { get; init; }

	public static RenderSkin Create(RenderMesh source)
	{
		var vertices = RenderMesh.VertexBuffer.Allocate(source.VertexHandle.Size);

		return new RenderSkin()
		{
			Vertices = vertices,
			BLAS = new BottomLevelAS(RenderMesh.VertexBuffer, vertices.Offset, vertices.Size,
				RenderMesh.IndexBuffer, source.IndexHandle.Offset, source.IndexHandle.Size, allowUpdate: true),
		};
	}

	public void Dispose()
	{
		Vertices.Dispose();
		BLAS.Dispose();
	}
}

[StructLayout(LayoutKind.Sequential)]
internal struct MeshData
{
	public required uint VertexOffset; // Start of vertices in vertex buffer
	public required uint IndexOffset; // Start of indices in index buffer
	public required uint IndexCount; // Number of indices in index buffer
}