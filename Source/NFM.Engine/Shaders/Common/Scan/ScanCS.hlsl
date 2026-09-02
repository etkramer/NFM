#include "Shaders/Common/Scan/Scan.h"

StructuredBuffer<uint> Counts : register(t0);

RWStructuredBuffer<uint> Offsets : register(u0);
RWStructuredBuffer<uint> BlockSums : register(u1);

cbuffer Constants : register(b0)
{
	int Count;
}

// Exclusive scan within each block, leaving the block's total for the next level to fold in.
[numthreads(SCAN_GROUP_SIZE, 1, 1)]
void main(uint3 groupID : SV_GroupID, uint threadID : SV_GroupIndex)
{
	uint index = groupID.x * SCAN_GROUP_SIZE + threadID;
	uint value = index < (uint)Count ? Counts[index] : 0;

	Partials[threadID] = value;
	GroupMemoryBarrierWithGroupSync();

	ScanPartials(threadID);

	if (index < (uint)Count)
	{
		Offsets[index] = Partials[threadID] - value;
	}

	if (threadID == SCAN_GROUP_SIZE - 1)
	{
		BlockSums[groupID.x] = Partials[threadID];
	}
}
