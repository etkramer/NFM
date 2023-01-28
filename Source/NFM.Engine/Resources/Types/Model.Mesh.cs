using System;
using NFM.GPU;
using NFM.Graphics;

namespace NFM.Resources;

/// <summary>
/// A part of a model that contains geometry - each model must have at least one of these per unique material.
/// </summary>
public partial class Mesh : IDisposable
{
	// Geometry buffers
	internal static GraphicsBuffer<uint> IndexBuffer = new(20000000 * 3); // Support 20m tris
	internal static GraphicsBuffer<Vertex> VertexBuffer = new(20000000); // Support 20m verts
	internal static GraphicsBuffer<Meshlet> MeshletBuffer = new(20000000); // Support 20m meshlets
	internal static GraphicsBuffer<GPUMesh> MeshBuffer = new(20000000 + 1); // Support 20m meshes

	static Mesh()
	{
		MeshBuffer.Name = "Mesh Buffer";
		MeshBuffer.Allocate(1, true); // First element is reserved to represent an invalid index.
	}

	// Geometry allocations
	internal BufferAllocation<uint> IndexHandle;
	internal BufferAllocation<Vertex> VertexHandle;
	internal BufferAllocation<Meshlet> MeshletHandle;
	internal BufferAllocation<GPUMesh> MeshHandle;

	/// <summary>
	/// Commits all changes to mesh data - must be called at least once before use.
	/// </summary>
	public unsafe void Commit()
	{
		if (IsCommitted)
		{
			return;
		}

		// Free existing allocations.
		VertexHandle?.Dispose();
		IndexHandle?.Dispose();
		MeshHandle?.Dispose();
		MeshletHandle?.Dispose();

		if (Bounds == Box3D.Infinity)
		{
			Bounds = CalculateBounds();
		}

		fixed (uint* indicesPtr = Indices)
		{
			// Build meshlet data
			MeshOptimizer.BuildMeshlets(Indices, Vertices.Length, out var meshletIndices, out var meshletVerts, out var meshlets);

			// Remap vertices to match meshlet output
			Vertex[] verts = new Vertex[meshletVerts.Length];
			for (int i = 0; i < meshletVerts.Length; i++)
			{
				verts[i] = Vertices[meshletVerts[i]];
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

		// Mark mesh as committed.
		IsCommitted = true;
	}

	public Box3D CalculateBounds()
	{
		Vector3 min = Vector3.PositiveInfinity;
		Vector3 max = Vector3.NegativeInfinity;

		// Nothing fancy, just loop over every vert and
		// compare to the current min/max values.
		for (int i = 0; i < Vertices.Length; i++)
		{
			var vert = Vertices[i];

			// Update minimums.
			if (vert.Position.X < min.X)
			{
				min.X = vert.Position.X;
			}
			if (vert.Position.Y < min.Y)
			{
				min.Y = vert.Position.Y;
			}
			if (vert.Position.Z < min.Z)
			{
				min.Z = vert.Position.Z;
			}

			// Update maximums.
			if (vert.Position.X > max.X)
			{
				max.X = vert.Position.X;
			}
			if (vert.Position.Y > max.Y)
			{
				max.Y = vert.Position.Y;
			}
			if (vert.Position.Z > max.Z)
			{
				max.Z = vert.Position.Z;
			}
		}

		return new Box3D(min, max);
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
public unsafe struct Vertex
{
	public Vector3 Position;
	public Vector3 Normal;
	public Vector2 UV0;
	public Vector2 UV1;
}

[StructLayout(LayoutKind.Sequential)]
internal struct GPUMesh
{
	public required uint VertexOffset; // Start of submesh in vertex buffer.
	public required uint IndexOffset; // Start of submesh in index buffer.
	public required uint MeshletOffset; // Start of submesh in meshlet buffer.
	public required uint MeshletCount;   // Number of meshlets used
}