#include "Shaders/Common/Scan/Scan.h"

StructuredBuffer<uint> BlockSums : register(t0);

RWStructuredBuffer<uint> Offsets : register(u0);

cbuffer Constants : register(b0)
{
	int Count;
}

// Lifts each block's offsets by everything scanned before it.
[numthreads(SCAN_GROUP_SIZE, 1, 1)]
void main(uint3 groupID : SV_GroupID, uint threadID : SV_GroupIndex)
{
	uint index = groupID.x * SCAN_GROUP_SIZE + threadID;
	if (index < (uint)Count)
	{
		Offsets[index] += BlockSums[groupID.x];
	}
}
