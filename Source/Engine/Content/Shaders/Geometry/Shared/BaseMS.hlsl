#include "Content/Shaders/Geometry/Shared/BaseMS.h"

cbuffer CommandConstants : register(b0)
{
	int InstanceID;
}

[NumThreads(124, 1, 1)]
[OutputTopology("triangle")]
void BaseMS(uint groupID : SV_GroupID, uint groupThreadID : SV_GroupThreadID, out vertices VertAttribute outVerts[64], out primitives PrimAttribute outPrims[124], out indices uint3 outIndices[124])
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
		normal = normalize(mul(normal.xyz, (float3x3)transform.WorldToObject));

		// Write output vertex.
		outVerts[groupThreadID].Position = position;
		outVerts[groupThreadID].Normal = float4(normal, 1);
		outVerts[groupThreadID].UV0 = vertex.UV0;
	}
	if (groupThreadID < meshlet.PrimCount)
	{
		// Write output triangle.
		outIndices[groupThreadID] = GetPrimitive(mesh, meshlet, groupThreadID);
		outPrims[groupThreadID].PrimitiveID = groupThreadID;
		outPrims[groupThreadID].InstanceID = InstanceID;
		outPrims[groupThreadID].MeshletID = groupID;
	}
}