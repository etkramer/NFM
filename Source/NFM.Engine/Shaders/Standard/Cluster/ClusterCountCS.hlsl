#include "Shaders/Clustering.h"

RWStructuredBuffer<uint> ClusterCounts : register(u0);

cbuffer Constants : register(b0)
{
	int2 FrameSize;
	int GroupsPerLight;
}

// A grid of groups per light slot, striding over the clusters that light reaches.
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

		uint ignored;
		InterlockedAdd(ClusterCounts[ClusterIndex(coord)], 1, ignored);
	}
}
