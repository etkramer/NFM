#include "Content/Shaders/Geometry/Geometry.h"
#include "Content/Shaders/Gizmos/Gizmos.h"

cbuffer InputConstants : register(b0)
{
	float3 Pos0 : packoffset(c0);
	float3 Pos1 : packoffset(c1);
	float3 Color : packoffset(c2);
};

float4 ToClipSpace(float3 pt)
{
	float4 result = float4(pt, 1);
	result = mul(ViewConstants.WorldToView, result);
	result = mul(ViewConstants.ViewToClip, result);

	return result;
}

[NumThreads(1, 1, 1)]
[OutputTopology("line")]
void LineMS(out vertices VertexAttribute outVerts[2], out indices uint2 outIndices[1])
{
	SetMeshOutputCounts(2, 1);

	// Create vertex attributes.
	VertexAttribute vert0, vert1;
	vert0.Color = float4(Color, 1);
	vert1.Color = float4(Color, 1);

	// Calculate clip space coords.
	float4 clip0 = ToClipSpace(Pos0);
	float4 clip1 = ToClipSpace(Pos1);

	vert0.Position = clip0;
	vert1.Position = clip1;

	// Output vertices/indices
	outVerts[0] = vert0; outVerts[1] = vert1;
	outIndices[0] = uint2(0, 1);
}