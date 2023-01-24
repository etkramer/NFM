#include "Shaders/Common.h"
#include "Shaders/Standard/Geometry/BaseMS.h"

uint2 VisibilityPS(VertAttribute vert, PrimAttribute prim) : SV_TARGET0
{
	// Instance ID: 32 bits
	// Meshlet ID: 25 bits
	// Triangle ID: 7 bits (<124 tris/meshlet)

	return uint2(PackBits(prim.MeshletID, prim.PrimitiveID, 25), prim.InstanceID);
}