#include "HLSL/BaseMS.h"

cbuffer CommandConstants : register(b0)
{
	int InstanceID;
}

[NumThreads(124, 1, 1)]
[OutputTopology("triangle")]
void MeshEntry(uint groupID : SV_GroupID, uint groupThreadID : SV_GroupThreadID, out primitives PrimAttribute prims[124], out indices uint3 indices[124], out vertices VertAttribute verts[64])
{
	// Grab instance data.
	Instance instance = Instances[InstanceID];
	Mesh mesh = Meshes[instance.MeshID];
	Meshlet meshlet = Meshlets[mesh.MeshletStart + groupID];

	// Set the meshlet output counts.
	SetMeshOutputCounts(meshlet.VertCount, meshlet.PrimCount);

	// Write vertex/index data.
	if (groupThreadID < meshlet.VertCount)
	{
		// Fetch vertex data.
		Vertex vertex = GetVertex(mesh, meshlet, groupThreadID);

		// Grab the transform matrix.
		Transform transform = Transforms[instance.TransformID];

		// Apply projection to vertex position.
		float4 position = float4(vertex.Position, 1);
		position = mul(transform.ObjectToWorld, position); // Apply instance transform.
		position = mul(ViewConstants.WorldToView, position); // Apply camera view.
		position = mul(ViewConstants.ViewToClip, position); // Apply camera projection.

		// Apply transformation to vertex normal.
		float3 normal = vertex.Normal;
		normal = mul(normal.xyz, (float3x3)transform.WorldToObject);

		// Write output vertex.
		verts[groupThreadID].Position = position;
		verts[groupThreadID].Normal = float4(normal, 1);
	}
	if (groupThreadID < meshlet.PrimCount)
	{
		// Write output triangle.
		indices[groupThreadID] = GetPrimitive(mesh, meshlet, groupThreadID);
		prims[groupThreadID].PrimitiveID = groupThreadID;
		prims[groupThreadID].InstanceID = InstanceID;
		prims[groupThreadID].MeshletID = groupID;
	}
}