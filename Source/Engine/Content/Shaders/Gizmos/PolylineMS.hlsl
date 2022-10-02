#include "Content/Shaders/Include/Geometry.h"
#include "Content/Shaders/Gizmos/Gizmos.h"

cbuffer InputConstants : register(b0)
{
	float3 Pos0 : packoffset(c0);
	float3 Pos1 : packoffset(c1);
	float3 Color : packoffset(c2);
	int Width;
};

float4 ToClipSpace(float3 pt)
{
	float4 result = float4(pt, 1);
	result = mul(ViewConstants.WorldToView, result);
	result = mul(ViewConstants.ViewToClip, result);

	return result;
}

/*[NumThreads(1, 1, 1)]
[OutputTopology("triangle")]
void PolylineMS(out indices uint3 indices[2], out vertices VertexAttribute verts[4])
{
	SetMeshOutputCounts(4, 2);

	// Create vertex attributes.
	VertexAttribute vert0, vert1, vert2, vert3;
	vert0.Color = float4(Color, 1);
	vert1.Color = float4(Color, 1);
	vert2.Color = float4(Color, 1);
	vert3.Color = float4(Color, 1);

	// Calculate clip space coords.
	float4 clip0 = ToClipSpace(Pos0);
	float4 clip1 = ToClipSpace(Pos1);

	// Calculate NDC coords.
	float aspect = (ViewConstants.ViewportSize.x / ViewConstants.ViewportSize.y);
	float2 ndc0 = clip0.xy / clip0.w;
	float2 ndc1 = clip1.xy / clip1.w;
	ndc0.x *= aspect;
	ndc1.x *= aspect;

	// Calculate line normal.
	float2 dir = normalize(ndc1 - ndc0);
	float2 normal = float2(-dir.y, dir.x);

	// Extrude from center and correct aspect ratio.
	normal *= Width / 2.0;
	normal.x /= aspect;

	// Offset by the direction of this point in the pair (-1 or 1)
	float4 offset0 = float4(normal * -1, 0.0, 0.0);
	float4 offset1 = float4(normal, 0.0, 0.0);

	vert0.Position = (clip0 + offset0);
	vert1.Position = (clip0 + offset1);
	vert2.Position = (clip1 + offset0);
	vert3.Position = (clip1 + offset1);

	// Output vertices/indices
	verts[0] = vert0; verts[1] = vert1; verts[2] = vert2; verts[3] = vert3;
	indices[0] = uint3(0, 3, 2); indices[1] = uint3(0, 2, 1);
}*/

[NumThreads(1, 1, 1)]
[OutputTopology("line")]
void PolylineMS(out indices uint2 indices[1], out vertices VertexAttribute verts[2])
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
	verts[0] = vert0; verts[1] = vert1;
	indices[0] = uint2(0, 1);
}