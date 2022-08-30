#include "HLSL/Include/Common.h"
#include "HLSL/Material/BaseMaterialPS.h"
#include "HLSL/BaseMS.h"

ByteAddressBuffer MaterialParams : register(t0);

#insert MATERIAL

float4 MaterialPS(VertAttribute vert, PrimAttribute prim) : SV_TARGET0
{
	// Read instance data from MS outputs.
	Instance inst = Instances[prim.InstanceID];
	uint materialID = inst.MaterialID;

	// Read material params from buffer.
	uint shaderID = MaterialParams.Load(materialID + 0);

	#insert SETUP

	// Get params from surface shader.
	SurfaceInfo surfaceInfo = SurfaceMain();

	// Calculate normal vector from combined vert/surface normals.
	float3 normal = vert.Normal.xyz * 0.5 + 0.5;

	return float4(normal, 1);
}