using System;
using NFM.GPU;
using NFM.Graphics;

namespace NFM.Resources;

public struct Meshlet
{
	public uint vertexOffset;
	public uint triangleOffset;

	public uint vertexCount;
	public uint triangleCount;
};

static unsafe class MeshOptimizer
{
	[DllImport("meshoptimizer.dll")]
	static extern ulong buildMeshletsBound(ulong index_count, ulong max_vertices, ulong max_triangles);

	[DllImport("meshoptimizer.dll")]
	static extern ulong buildMeshletsScan(Meshlet* meshlets, uint* meshlet_vertices, byte* meshlet_triangles, uint* indices, ulong index_count, ulong vertex_count, ulong max_vertices, ulong max_triangles);
	
	public static void BuildMeshlets(Span<uint> indices, int numVerts, out Span<byte> outIndices, out Span<uint> outVertIndices, out Span<Meshlet> outMeshlets)
	{
		const ulong maxVerts = 64;
		const ulong maxTris = 124;

		ulong maxMeshlets = buildMeshletsBound((ulong)indices.Length, maxVerts, maxTris);

		var meshlets = new Meshlet[maxMeshlets];
		var meshletVerts = new uint[maxMeshlets * maxVerts];
		var meshletTris = new byte[maxMeshlets * maxVerts * 3];

		fixed (Meshlet* meshletsPtr = meshlets)
		fixed (uint* meshletVertsPtr = meshletVerts)
		fixed (byte* meshletTrisPtr = meshletTris)
		fixed (uint* indicesPtr = indices)
		{
			ulong meshletCount = buildMeshletsScan(meshletsPtr, meshletVertsPtr, meshletTrisPtr, indicesPtr, (ulong)indices.Length, (ulong)numVerts, maxVerts, maxTris);

			Meshlet last = meshlets[meshletCount - 1];
			ulong meshletSize = meshletCount;
			ulong meshletVertsSize = last.vertexOffset + last.vertexCount;
			ulong meshletTrisSize = (ulong)(last.triangleOffset + ((last.triangleCount * 3 + 3) & ~3));

			outMeshlets = meshlets.AsSpan().Slice(0, (int)meshletSize);
			outVertIndices = meshletVerts.AsSpan().Slice(0, (int)meshletVertsSize);
			outIndices = meshletTris.AsSpan().Slice(0, (int)meshletTrisSize);
		}
	}
}