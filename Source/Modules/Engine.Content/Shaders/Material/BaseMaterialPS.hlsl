#include "Shaders/BaseMS.h"
#include "Shaders/Include/Common.h"

#insert MATERIAL

float4 PixelEntry(VertAttribute vert, PrimAttribute prim) : SV_TARGET
{
	uint instanceID = prim.InstanceID; // 32 bits allows for ~4b instances
	uint meshletID = prim.MeshletID; // (32-7) bits allows for ~33m meshlets (~4b tris/instance)
	uint primID = prim.PrimitiveID;

	// G-buffer layout:
	//	Color map (32-bit):
	//		RGB: Color
	//		A: Opacity
	//	NRM map (32-bit):
	//		RG: Normal
	//		B: Roughness
	//		A: Metallic

	SurfaceInfo surfaceInfo = Surface();

	return float4(vert.Normal);
}