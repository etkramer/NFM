#include "HLSL/Include/Common.h"
#include "HLSL/Material/BaseMaterialPS.h"
#include "HLSL/BaseMS.h"

struct SurfaceState
{
	Surface Defaults;
	float2 UV;
};

SamplerState DefaultSampler : register(s0);
ByteAddressBuffer MaterialParams : register(t0);

#insert SURFACE

float4 MaterialPS(VertAttribute vert, PrimAttribute prim) : SV_TARGET0
{
	// Read instance data from MS outputs.
	Instance inst = Instances[prim.InstanceID];
	uint materialID = inst.MaterialID;

	// Read material params from buffer.
	uint shaderID = MaterialParams.Load(materialID + 0);

	#insert SETUP

	// Build surface shader state.
	SurfaceState state;
	state.Defaults = GetDefaultSurface();
	state.UV = vert.UV0;

	// Get params from surface shader.
	Surface surface = SurfaceMain(state);

	// Calculate normal vector from combined vert/surface normals.
	float3 normal = vert.Normal.xyz / 2 + 0.5;

	//return float4(surface.Albedo.xyz, 1);
	return float4(LinearToSRGB(normal), 1);
}