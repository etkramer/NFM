RWStructuredBuffer<uint> ClusterCounts : register(u0);
RWStructuredBuffer<uint> ClusterCursors : register(u1);

cbuffer Constants : register(b0)
{
	int ClusterCount;
}

[numthreads(64, 1, 1)]
void main(uint3 id : SV_DispatchThreadID)
{
	if (id.x >= (uint)ClusterCount)
	{
		return;
	}

	ClusterCounts[id.x] = 0;
	ClusterCursors[id.x] = 0;
}
