#include "Shaders/Clustering.h"

StructuredBuffer<uint> ClusterOffsets : register(t0);

RWStructuredBuffer<uint> ClusterCursors : register(u0);
RWStructuredBuffer<uint> ClusterLights : register(u1);

cbuffer Constants : register(b0)
{
	int2 FrameSize;
	int GroupsPerLight;
}

// Walks the same clusters the count pass did, filling each one's slice of the shared pool.
[numthreads(64, 1, 1)]
void main(uint3 groupID : SV_GroupID, uint threadID : SV_GroupIndex)
{
	uint3 lo, hi;
	if (!LightClusterBounds(Lights[groupID.y], FrameSize, lo, hi))
	{
		return;
	}

	uint3 extent = hi - lo + 1;
	uint total = extent.x * extent.y * extent.z;
	uint stride = GroupsPerLight * 64;

	for (uint i = groupID.x * 64 + threadID; i < total; i += stride)
	{
		uint3 coord = lo + uint3(i % extent.x, (i / extent.x) % extent.y, i / (extent.x * extent.y));
		uint cluster = ClusterIndex(coord);

		uint slot;
		InterlockedAdd(ClusterCursors[cluster], 1, slot);

		uint index = ClusterOffsets[cluster] + slot;
		if (index < CLUSTER_LIGHT_POOL)
		{
			ClusterLights[index] = groupID.y;
		}
	}
}
