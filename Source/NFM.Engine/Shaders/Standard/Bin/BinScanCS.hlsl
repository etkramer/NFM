#define MAX_MATERIAL_STACKS 256
#define MATERIAL_GROUP_SIZE 64

RWStructuredBuffer<uint> BinCounts : register(u0);
RWStructuredBuffer<uint> BinOffsets : register(u1);
RWStructuredBuffer<uint> BinCursors : register(u2);
RWStructuredBuffer<uint3> BinDispatchArgs : register(u3);

[numthreads(1, 1, 1)]
void main()
{
	uint total = 0;

	for (uint i = 0; i < MAX_MATERIAL_STACKS; i++)
	{
		uint count = BinCounts[i];

		BinOffsets[i] = total;
		BinCursors[i] = 0;
		BinDispatchArgs[i] = uint3((count + MATERIAL_GROUP_SIZE - 1) / MATERIAL_GROUP_SIZE, 1, 1);

		total += count;
	}
}
