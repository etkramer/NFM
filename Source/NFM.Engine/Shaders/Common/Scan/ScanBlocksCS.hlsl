#include "Shaders/Common/Scan/Scan.h"

RWStructuredBuffer<uint> BlockSums : register(u0);

cbuffer Constants : register(b0)
{
	int Count;
}

// Exclusive scan over the block totals, in one group, each thread reducing a contiguous chunk.
[numthreads(SCAN_GROUP_SIZE, 1, 1)]
void main(uint threadID : SV_GroupIndex)
{
	uint chunk = ((uint)Count + SCAN_GROUP_SIZE - 1) / SCAN_GROUP_SIZE;
	uint start = min(threadID * chunk, (uint)Count);
	uint end = min(start + chunk, (uint)Count);

	uint total = 0;
	for (uint i = start; i < end; i++)
	{
		total += BlockSums[i];
	}

	Partials[threadID] = total;
	GroupMemoryBarrierWithGroupSync();

	ScanPartials(threadID);

	uint running = Partials[threadID] - total;
	for (uint j = start; j < end; j++)
	{
		uint value = BlockSums[j];
		BlockSums[j] = running;
		running += value;
	}
}
