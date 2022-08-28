#include "HLSL/Include/Common.h"
#include "HLSL/Material/BaseMaterialPS.h"
#include "HLSL/BaseMS.h"

ByteAddressBuffer MaterialParams : register(t0);

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
	uint instanceID = prim.InstanceID;
	uint materialID = Instances[instanceID].MaterialID;

	// Read material params from buffer.
	uint shaderID = MaterialParams.Load(materialID + 0);

	// Get params from surface shader.
	SurfaceInfo surfaceInfo = SurfaceMain();

	// Calculate normal vector from combined vert/surface normals.
	float2 normal = vert.Normal.xy * 0.5 + 0.5;

	// Copy to G-buffer targets.
	PSTargets targets;
	targets.Color = float4(surfaceInfo.Color, surfaceInfo.Emission);
	targets.NRM = float4(normal.xy, surfaceInfo.Roughness, surfaceInfo.Metallic);

	targets.Color = float4(IndexToColor(shaderID), 1);

	return targets;
}