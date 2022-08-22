#include "Shaders/Include/Geometry.hlsl"
#include "Shaders/Include/Outputs.hlsl"
#include "Shaders/Include/Common.hlsl"

uint2 PixelEntry(VertAttribute vert, PrimAttribute prim) : SV_TARGET
{
	uint instanceID = vert.InstanceID; // 32 bits allows for ~4b instances
	uint meshletID = vert.MeshletID; // (32-7) bits allows for ~33m meshlets (~4b tris/instance)
	uint primID = prim.PrimitiveID;

	return uint2(instanceID, BitPack(meshletID, primID, 25));
}