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
		internal static GraphicsBuffer<GPUMesh> MeshBuffer = new(1000000 + 1);

		static Mesh()
		{
			MeshBuffer.Name = "Mesh Buffer";
			MeshBuffer.Allocate(1, true); // First element is reserved to represent an invalid index.
		}

		// Geometry allocations
		internal BufferAllocation<uint> PrimHandle;
		internal BufferAllocation<Vertex> VertHandle;
		internal BufferAllocation<Meshlet> MeshletHandle;
		internal BufferAllocation<GPUMesh> MeshHandle;

		public bool IsCommitted { get; private set; } = false;

		public Box3D Bounds { get; set; } = Box3D.Infinity;

		public uint[] Indices { get; set; }
		public Vertex[] Vertices { get; set; }
		public Material Material { get; set; }

		public void SetIndices(uint[] value)
		{
			Indices = value;
			IsCommitted = false;
		}

		public void SetVertices(Vertex[] value)
		{
			Vertices = value;
			IsCommitted = false;
		}

		public void SetMaterial(Material value)
		{
			Material = value;
			IsCommitted = false;
		}

		/// <summary>
		/// Commits all changes to mesh data - must be called at least once before use.
		/// </summary>
		public unsafe void Commit()
		{
			// Free existing allocations.
			VertHandle?.Dispose();
			PrimHandle?.Dispose();
			MeshHandle?.Dispose();
			MeshletHandle?.Dispose();

			if (Bounds == Box3D.Infinity)
			{
				Bounds = CalculateBounds();
			}

			fixed (uint* indicesPtr = Indices)
			{
				// Build meshlet data.
				MeshOperations.BuildMeshlets(indicesPtr, Indices.Length, Vertices.Length, out var meshletPrims, out var meshletVerts, out var meshlets);

				// Remap vertices to match meshlet output.
				Vertex[] verts = new Vertex[meshletVerts.Length];
				for (int i = 0; i < meshletVerts.Length; i++)
				{
					verts[i] = Vertices[meshletVerts[i]];
				}

				// Upload geometry data to GPU.
				VertHandle = VertBuffer.Allocate(verts.Length);
				PrimHandle = PrimBuffer.Allocate(meshletPrims.Length);
				MeshletHandle = MeshletBuffer.Allocate(meshlets.Length);
				Renderer.DefaultCommandList.UploadBuffer(VertHandle, verts);
				Renderer.DefaultCommandList.UploadBuffer(PrimHandle, meshletPrims.Select(o => (uint)o).ToArray());
				Renderer.DefaultCommandList.UploadBuffer(MeshletHandle, meshlets);

				// Upload mesh info to GPU.
				MeshHandle = MeshBuffer.Allocate(1);
				Renderer.DefaultCommandList.UploadBuffer(MeshHandle, new GPUMesh()
				{
					MeshletCount = (uint)MeshletHandle.Size,
					MeshletOffset = (uint)MeshletHandle.Offset,
					PrimOffset = (uint)PrimHandle.Offset,
					VertOffset = (uint)VertHandle.Offset,
				});
			}

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
			PrimHandle?.Dispose();
			VertHandle?.Dispose();
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
		public uint VertOffset; // Start of submesh in vertex buffer.
		public uint PrimOffset; // Start of submesh in primitive buffer.
		public uint MeshletOffset; // Start of submesh in meshlet buffer.
		public uint MeshletCount;   // Number of meshlets used
	}
}