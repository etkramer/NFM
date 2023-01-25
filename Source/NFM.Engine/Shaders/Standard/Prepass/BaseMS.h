#include "../../World.h"

struct PrimAttribute
{
	uint InstanceID : INSTANCEID;
	uint MeshletID : MESHLETID;
	uint TriangleID : TRIANGLEID;
};

uint3 GetPrimitive(uint offset)
{
	return uint3(Primitives[offset], Primitives[offset + 1], Primitives[offset + 2]);
}