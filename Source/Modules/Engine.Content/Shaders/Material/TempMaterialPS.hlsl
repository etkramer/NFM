#include "Shaders/BaseMS.h"
#include "Shaders/Include/Common.h"

float4 PixelEntry(VertAttribute vert, PrimAttribute prim) : SV_TARGET
{
	uint instanceID = prim.InstanceID; // 32 bits allows for ~4b instances
	uint meshletID = prim.MeshletID; // (32-7) bits allows for ~33m meshlets (~4b tris/instance)
	uint primID = prim.PrimitiveID;

	return float4(vert.Normal);
}