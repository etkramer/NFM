#include "Content/Shaders/Gizmos/Gizmos.h"

StructuredBuffer<float4> Vertices : register(t0);
StructuredBuffer<uint3> Indices : register(t1);

float4 Color : register(b0);

[NumThreads(1, 1, 1)]
[OutputTopology("triangle")]
void GeomMS(uint dispatchThreadID : SV_DispatchThreadID, out vertices VertexAttribute outVerts[3], out indices uint3 outIndices[1])
{
	SetMeshOutputCounts(3, 1);

	// Create vertex attributes.
	VertexAttribute vert0, vert1, vert2;
	vert0.Color = Color;
	vert1.Color = Color;
	vert2.Color = Color;
	
	uint3 vertIndices = Indices[dispatchThreadID];
	vert0.Position = Vertices[vertIndices.x];
	vert1.Position = Vertices[vertIndices.y];
	vert2.Position = Vertices[vertIndices.z];

	// Output vertices/indices
	outVerts[0] = vert0;
	outVerts[1] = vert1;
	outVerts[2] = vert2;
	outIndices[0] = uint3(0, 1, 2);
}