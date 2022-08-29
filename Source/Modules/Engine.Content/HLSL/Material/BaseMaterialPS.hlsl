#include "HLSL/Include/Common.h"
#include "HLSL/Material/BaseMaterialPS.h"
#include "HLSL/BaseMS.h"

ByteAddressBuffer MaterialParams : register(t0);

#insert MATERIAL

struct PSTargets
{
	float4 Color : SV_TARGET0;
	float4 NRM : SV_TARGET1;
};

PSTargets MaterialPS(VertAttribute vert, PrimAttribute prim)
{
	// G-buffer layout:
	//	Color map (32-bit):
	//		RGB: Color
	//		A: Emission
	//	NRM map (32-bit):
	//		RG: Normal
	//		B: Roughness
	//		A: Metallic

	// Read instance data from MS outputs.
	Instance inst = Instances[prim.InstanceID];
	uint materialID = inst.MaterialID;

	// Read material params from buffer.
	uint shaderID = MaterialParams.Load(materialID + 0);
	#insert SETUP

	// Get params from surface shader.
	SurfaceInfo surfaceInfo = SurfaceMain();

	// Calculate normal vector from combined vert/surface normals.
	float2 normal = vert.Normal.xy * 0.5 + 0.5;

	// Copy to G-buffer targets.
	PSTargets targets;
	targets.Color = float4(surfaceInfo.Color, surfaceInfo.Emission);
	targets.NRM = float4(normal.xy, surfaceInfo.Roughness, surfaceInfo.Metallic);

	return targets;
}