#define MAX_MATERIAL_STACKS 256
#define MATERIAL_GROUP_SIZE 64

RWStructuredBuffer<uint> BinCounts : register(u0);
RWStructuredBuffer<uint> BinOffsets : register(u1);
RWStructuredBuffer<uint> BinCursors : register(u2);
RWStructuredBuffer<uint3> BinDispatchArgs : register(u3);

groupshared uint Partials[MAX_MATERIAL_STACKS];

[numthreads(MAX_MATERIAL_STACKS, 1, 1)]
void main(uint id : SV_GroupIndex)
{
	uint count = BinCounts[id];
	Partials[id] = count;
	GroupMemoryBarrierWithGroupSync();

	// Inclusive scan over the per-stack counts.
	for (uint offset = 1; offset < MAX_MATERIAL_STACKS; offset <<= 1)
	{
		uint sum = Partials[id];
		if (id >= offset)
		{
			sum += Partials[id - offset];
		}

		GroupMemoryBarrierWithGroupSync();
		Partials[id] = sum;
		GroupMemoryBarrierWithGroupSync();
	}

	// Each stack starts where the one before it ended.
	BinOffsets[id] = Partials[id] - count;
	BinCursors[id] = 0;
	BinDispatchArgs[id] = uint3((count + MATERIAL_GROUP_SIZE - 1) / MATERIAL_GROUP_SIZE, 1, 1);
}
