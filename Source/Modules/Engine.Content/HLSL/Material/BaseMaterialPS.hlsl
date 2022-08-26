#include "HLSL/Include/Common.h"
#include "HLSL/Material/BaseMaterialPS.h"
#include "HLSL/BaseMS.h"

//#insert MATERIAL
SurfaceInfo SurfaceMain()
{
	SurfaceInfo surface;
	surface.Color = float3(1, 0, 1);
	surface.Normal = float3(0.5, 0.5, 1);
	surface.Metallic = 0;
	surface.Roughness = 0.5;

	return surface;
}

struct PSTargets
{
	float4 Color : SV_TARGET0;
	float4 NRM : SV_TARGET1;
};

float4 MaterialPS(VertAttribute vert, PrimAttribute prim) : SV_TARGET
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
	uint instanceID = prim.InstanceID;
	uint materialID = Instances[instanceID].MaterialID;

	// Get params from surface shader.
	SurfaceInfo surfaceInfo = SurfaceMain();

	// Copy to G-buffer targets.
	PSTargets targets;
	targets.Color = float4(surfaceInfo.Color, surfaceInfo.Emission);
	targets.NRM = float4(surfaceInfo.Normal.xy, surfaceInfo.Roughness, surfaceInfo.Metallic);

	return float4(vert.Normal);
}