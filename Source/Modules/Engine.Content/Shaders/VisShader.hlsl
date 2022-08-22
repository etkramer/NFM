#include "Shaders/Include/Geometry.hlsl"
#include "Shaders/Include/Outputs.hlsl"
#include "Shaders/Include/Common.hlsl"

uint2 PixelEntry(MeshOut input) : SV_TARGET
{
	uint instanceID = input.InstanceID; // 32 bits allows for ~4b instances
	uint meshletID = input.MeshletID; // (32-7) bits allows for ~33m meshlets (~4b tris)
	uint triangleID = input.TriangleID; // Needs exactly 7 bits to represent up to 128 tris.

	return uint2(instanceID, BitPack(meshletID, triangleID, 25));
}