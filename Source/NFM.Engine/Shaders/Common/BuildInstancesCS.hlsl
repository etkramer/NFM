#include "Shaders/World.h"

// Mirrors D3D12_RAYTRACING_INSTANCE_DESC.
struct RTInstance
{
	float4 Transform[3];
	uint InstanceIDAndMask;
	uint ContributionAndFlags;
	uint2 BLASAddress;
};

RWStructuredBuffer<RTInstance> RTInstances : register(u0);

cbuffer Constants : register(b0)
{
	int InstanceCount;
}

[numthreads(64, 1, 1)]
void main(uint id : SV_DispatchThreadID)
{
	if (id >= (uint)InstanceCount)
	{
		return;
	}

	Instance instance = Instances[id];
	RTInstance result = (RTInstance)0;

	// A zeroed mask leaves freed slots inactive, so the sparse instance buffer needs no compaction.
	if (instance.BLASAddress.x != 0 || instance.BLASAddress.y != 0)
	{
		float4x4 objectToWorld = Transforms[instance.TransformID].ObjectToWorld;

		result.Transform[0] = objectToWorld[0];
		result.Transform[1] = objectToWorld[1];
		result.Transform[2] = objectToWorld[2];

		result.InstanceIDAndMask = id | (0xFF << 24);
		result.BLASAddress = instance.BLASAddress;
	}

	RTInstances[id] = result;
}
