#include "Shaders/Common/Gizmos/Gizmos.h"

StructuredBuffer<GizmoVert> GizmoVertices : register(t0);
StructuredBuffer<uint> GizmoIndices : register(t1);

[NumThreads(1, 1, 1)]
[OutputTopology("triangle")]
void main(uint groupID : SV_GroupID, out vertices GizmoVertex outVerts[3], out indices uint3 outIndices[1])
{
	SetMeshOutputCounts(3, 1);

	[unroll]
	for (uint i = 0; i < 3; i++)
	{
		GizmoVert source = GizmoVertices[GizmoIndices[(groupID * 3) + i]];

		GizmoVertex vert;
		vert.Color = source.Color;
		vert.Position = ToClipSpace(source.Position);

		// Solid geometry covers every pixel it touches, so park the coverage test well inside the extent.
		vert.Coord = 0;
		vert.Extent = 1e6;

		outVerts[i] = vert;
	}

	outIndices[0] = uint3(0, 1, 2);
}
