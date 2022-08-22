#include "Shaders/Include/Outputs.hlsl"
#include "Shaders/Include/Geometry.hlsl"

cbuffer drawConstants : register(b0)
{
	int InstanceID;
}

cbuffer viewConstants : register(b1)
{
	float4x4 View;
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

[NumThreads(124, 1, 1)]
[OutputTopology("triangle")]
void MeshEntry(uint groupID : SV_GroupID, uint groupThreadID : SV_GroupThreadID, out primitives PrimAttribute prims[124], out indices uint3 indices[124], out vertices VertAttribute verts[64])
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
		position = mul(instance.Transform, position); // Apply instance transform.
		position = mul(View, position); // Apply camera view.
		position = mul(Projection, position); // Apply camera projection.

		// Write output vertex.
		verts[groupThreadID].Position = position;
		verts[groupThreadID].InstanceID = InstanceID;
		verts[groupThreadID].MeshletID = groupID;
	}
	if (groupThreadID < meshlet.PrimCount)
	{
		// Write output triangle.
		indices[groupThreadID] = GetPrimitive(mesh, meshlet, groupThreadID);
		prims[groupThreadID].PrimitiveID = groupThreadID;
	}
}