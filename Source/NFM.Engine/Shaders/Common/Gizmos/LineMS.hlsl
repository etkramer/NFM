#include "Shaders/Common/Gizmos/Gizmos.h"

StructuredBuffer<GizmoLine> Lines : register(t0);

cbuffer InputConstants : register(b0)
{
	float2 ViewportSize : packoffset(c0);
};

static const float NearW = 1e-4;
static const float EdgePad = 1;

[NumThreads(1, 1, 1)]
[OutputTopology("triangle")]
void main(uint groupID : SV_GroupID, out vertices GizmoVertex outVerts[4], out indices uint3 outIndices[2])
{
	GizmoLine segment = Lines[groupID];

	float4 clip0 = ToClipSpace(segment.P0);
	float4 clip1 = ToClipSpace(segment.P1);

	// Drop the segment when it's behind the eye entirely, and clip it when it straddles the near plane.
	bool visible = clip0.w >= NearW || clip1.w >= NearW;
	SetMeshOutputCounts(visible ? 4 : 0, visible ? 2 : 0);

	if (!visible)
	{
		return;
	}

	if (clip0.w < NearW)
	{
		clip0 = lerp(clip0, clip1, (NearW - clip0.w) / (clip1.w - clip0.w));
	}
	if (clip1.w < NearW)
	{
		clip1 = lerp(clip1, clip0, (NearW - clip1.w) / (clip0.w - clip1.w));
	}

	// Expand the segment into a screen-space quad, so width stays constant regardless of depth.
	float2 halfSize = ViewportSize * 0.5;
	float2 pixel0 = (clip0.xy / clip0.w) * halfSize;
	float2 pixel1 = (clip1.xy / clip1.w) * halfSize;

	float2 delta = pixel1 - pixel0;
	float length2D = length(delta);
	float2 along = length2D > 1e-5 ? delta / length2D : float2(1, 0);
	float2 across = float2(-along.y, along.x);

	float halfWidth = max(segment.Width, 0.5) * 0.5;
	float halfLength = length2D * 0.5;
	float2 center = (pixel0 + pixel1) * 0.5;

	// Caps are round, so the quad has to reach a full radius past either end.
	float reach = halfLength + halfWidth + EdgePad;
	float spread = halfWidth + EdgePad;

	[unroll]
	for (uint i = 0; i < 4; i++)
	{
		float2 corner = float2((i & 1) ? 1 : -1, (i & 2) ? 1 : -1);
		float2 offset = corner * float2(reach, spread);
		float4 clip = corner.x > 0 ? clip1 : clip0;

		GizmoVertex vert;
		vert.Color = segment.Color;
		vert.Coord = offset;
		vert.Extent = float2(halfLength, halfWidth);
		vert.Position = float4(((center + (along * offset.x) + (across * offset.y)) / halfSize) * clip.w, clip.z, clip.w);

		outVerts[i] = vert;
	}

	outIndices[0] = uint3(0, 1, 2);
	outIndices[1] = uint3(2, 1, 3);
}
