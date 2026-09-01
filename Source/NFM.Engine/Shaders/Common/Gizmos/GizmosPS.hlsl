#include "Shaders/Common/Gizmos/Gizmos.h"

float4 main(GizmoVertex vert) : SV_TARGET0
{
	// Distance to the primitive's solid core, faded over the last pixel to antialias the edge.
	float2 offset = float2(max(abs(vert.Coord.x) - vert.Extent.x, 0), vert.Coord.y);
	float alpha = vert.Color.a * saturate(vert.Extent.y + 0.5 - length(offset));

	// Premultiplied - the blend state scales only the destination.
	return float4(vert.Color.rgb * alpha, alpha);
}
