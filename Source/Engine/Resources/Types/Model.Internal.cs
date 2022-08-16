using System;
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
		internal Submesh[] Submeshes;

		internal unsafe void BuildSubmeshes()
		{
			// Submit submesh build tasks.
			List<Task<Submesh>> submeshBuildTasks = new();
			foreach (var bucket in Faces.Bucket((o) => o.Material))
			{
				submeshBuildTasks.Add(Task.Run(() =>
				{
					Material material = bucket.First().Material;
					Dictionary<uint, uint> indexMap = new();
				
					List<uint> submeshIndices = new(bucket.Count() * 3);
					List<Vertex> submeshVertices = new();

					// Build per-submesh vertex/index data.
					foreach (Face face in bucket)
					{
						// Create new verts and indices if needed.
						if (!indexMap.TryGetValue(face.A, out uint newIndexA))
						{
							submeshVertices.Add(new()
							{
								Position = Positions[face.A],
								Normal = Normals[face.A],
							});

							newIndexA = (uint)submeshVertices.Count - 1;
							indexMap.Add(face.A, newIndexA);
						}
						if (!indexMap.TryGetValue(face.B, out uint newIndexB))
						{
							submeshVertices.Add(new()
							{
								Position = Positions[face.B],
								Normal = Normals[face.B],
							});

							newIndexB = (uint)submeshVertices.Count - 1;
							indexMap.Add(face.B, newIndexB);
						}
						if (!indexMap.TryGetValue(face.C, out uint newIndexC))
						{
							submeshVertices.Add(new()
							{
								Position = Positions[face.C],
								Normal = Normals[face.C],
							});

							newIndexC = (uint)submeshVertices.Count - 1;
							indexMap.Add(face.C, newIndexC);
						}

						// Add the updated indices to the array.
						submeshIndices.Add(newIndexA);
						submeshIndices.Add(newIndexB);
						submeshIndices.Add(newIndexC);
					}

					fixed (uint* indicesPtr = submeshIndices.ToArray())
					{
						fixed (Vertex* verticesPtr = submeshVertices.ToArray())
						{
							// Build meshlet data.
							MeshOperations.BuildMeshlets(submeshIndices.Count, indicesPtr, submeshVertices.Count, verticesPtr, sizeof(Vertex), out var prims, out var verts, out var meshlets);

							// Build final vertex data.
							Vertex[] vertsData = new Vertex[verts.Length];
							for (int i = 0; i < verts.Length; i++)
							{
								vertsData[i] = submeshVertices[(int)verts[i]];
							}

							// Create submesh.
							Submesh submesh = new Submesh();  

							// Upload geometry data to GPU.
							Queue.Schedule(() =>
							{
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
							}, 0);

							return submesh;
						}
					}
				}));
			}

			// Wait for submeshes to be fully built.
			Task.WaitAll(submeshBuildTasks.ToArray());
			Submeshes = submeshBuildTasks.Select(o => o.Result).ToArray();
		}
	}

	internal class Submesh
	{
		public static GraphicsBuffer<uint> PrimBuffer = new(2000000);
		public static GraphicsBuffer<Vertex> VertBuffer = new(2000000);
		public static GraphicsBuffer<Meshlet> MeshletBuffer = new(2000000);
		public static GraphicsBuffer<Mesh> MeshBuffer = new(100000);

		public BufferHandle<uint> PrimHandle;
		public BufferHandle<Vertex> VertHandle;
		public BufferHandle<Meshlet> MeshletHandle;
		public BufferHandle<Mesh> MeshHandle;
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