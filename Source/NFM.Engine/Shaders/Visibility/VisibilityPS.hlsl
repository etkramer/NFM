#include "Shaders/Common.h"
#include "Shaders/Standard/Geometry/BaseMS.h"

uint2 VisibilityPS(VertAttribute vert, PrimAttribute prim) : SV_TARGET0
{
	return uint2(prim.InstanceID, PackBits(prim.MeshletID, prim.TriangleID, 25));
	//return uint2(prim.InstanceID, prim.TriangleID);
}