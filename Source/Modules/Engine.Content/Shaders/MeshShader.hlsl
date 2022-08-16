#include "Shaders/Include/Outputs.hlsl"
#include "Shaders/Include/Geometry.hlsl"

cbuffer drawConstants : register(b0)
{
	int InstanceID;
}

cbuffer viewConstants : register(b1)
{
	float4x4 Projection;
}

uint3 GetPrimitive(Mesh mesh, Meshlet meshlet, uint prim)
{
	uint prim0 = mesh.PrimStart + meshlet.PrimStart + (prim * 3);
	uint prim1 = prim0 + 1;
	uint prim2 = prim0 + 2;
	return uint3(Primitives[prim0], Primitives[prim1], Primitives[prim2]);
}

Vertex GetVertex(Mesh mesh, Meshlet meshlet, uint vert)
{
	return Vertices[mesh.VertStart + meshlet.VertStart + vert];
}

[NumThreads(128, 1, 1)]
[OutputTopology("triangle")]
void MeshEntry(uint groupID : SV_GroupID, uint groupThreadID : SV_GroupThreadID, out indices uint3 prims[128], out vertices MeshOut verts[64])
{
	// Grab instance data.
	Instance instance = Instances[InstanceID];
	Mesh mesh = Meshes[instance.Mesh];
	Meshlet meshlet = Meshlets[mesh.MeshletStart + groupID];

	// Set the meshlet output counts.
	SetMeshOutputCounts(meshlet.VertCount, meshlet.PrimCount);

	// Write vertex/index data.
	if (groupThreadID < meshlet.VertCount)
	{
		// Fetch vertex data.
		Vertex vertex = GetVertex(mesh, meshlet, groupThreadID);

		// Apply projection to vertex position.
		float4 position = float4(vertex.Position, 1);
		position = mul(instance.Transform, position);
		position = mul(Projection, position);

		// Write output vertex.
		verts[groupThreadID].InstanceID = InstanceID;
		verts[groupThreadID].MeshletID = groupID;
		verts[groupThreadID].TriangleID = groupThreadID;
		verts[groupThreadID].Position = position;
	}
	if (groupThreadID < meshlet.PrimCount)
	{
		// Write output triangle.
		prims[groupThreadID] = GetPrimitive(mesh, meshlet, groupThreadID);
	}
}