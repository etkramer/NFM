#include "Shaders/Standard/Prepass/BaseMS.h"

int InstanceID : register(b0);

[NumThreads(124, 1, 1)]
[OutputTopology("triangle")]
void BaseMS(uint groupID : SV_GroupID, uint groupThreadID : SV_GroupThreadID, out vertices float4 outVerts[64] : SV_POSITION, out primitives PrimAttribute outPrims[124], out indices uint3 outIndices[124])
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
		Vertex vertex = Vertices[mesh.VertStart + meshlet.VertStart + groupThreadID];

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
		outVerts[groupThreadID] = position;
	}
	if (groupThreadID < meshlet.PrimCount)
	{
		// Write output triangle.
		outPrims[groupThreadID].InstanceID = InstanceID;
		outPrims[groupThreadID].TriangleID = groupThreadID;
		outPrims[groupThreadID].MeshletID = groupID;
		outIndices[groupThreadID] = GetPrimitive(mesh.PrimStart + meshlet.PrimStart + (groupThreadID * 3));
	}
}