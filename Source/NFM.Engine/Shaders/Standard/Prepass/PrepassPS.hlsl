#include "Shaders/Common.h"
#include "Shaders/Standard/Prepass/BaseMS.h"

uint2 main(PrimAttribute prim) : SV_TARGET0
{
	return uint2(prim.InstanceID, PackBits(prim.MeshletID, prim.TriangleID, 25));
}