#include "../../World.h"

// Coord/Extent describe a pixel-space frame local to the primitive, used by the pixel shader to
// antialias edges. Coord is the offset from the primitive's center, Extent its solid half-size.
struct GizmoVertex
{
	float4 Color : COLOR;
	noperspective float2 Coord : TEXCOORD0;
	noperspective float2 Extent : TEXCOORD1;
	float4 Position : SV_POSITION;
};

struct GizmoLine
{
	float3 P0;
	float Width;
	float3 P1;
	float Padding;
	float4 Color;
};

struct GizmoVert
{
	float3 Position;
	float Padding;
	float4 Color;
};

float4 ToClipSpace(float3 pt)
{
	float4 result = float4(pt, 1);
	result = mul(ViewConstants.WorldToView, result);
	result = mul(ViewConstants.ViewToClip, result);

	return result;
}
