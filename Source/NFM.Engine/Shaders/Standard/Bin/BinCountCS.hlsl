#include "Shaders/World.h"

#define NO_STACK 0xFFFFFFFF

Texture2D<uint2> VisBuffer : register(t0);
Texture2D<float> DepthBuffer : register(t1);

ByteAddressBuffer MaterialParams : register(t0, space2);

RWStructuredBuffer<uint> BinCounts : register(u0);

[numthreads(8, 8, 1)]
void main(uint2 id : SV_DispatchThreadID)
{
	int2 frameSize;
	DepthBuffer.GetDimensions(frameSize.x, frameSize.y);

	if (id.x >= frameSize.x || id.y >= frameSize.y || DepthBuffer[id] == 0)
	{
		return;
	}

	uint materialID = Instances[VisBuffer[id].x].MaterialID;
	uint pending = MaterialParams.Load(materialID);

	// Each wave contributes a single atomic per distinct stack it covers, lanes retiring by mask.
	[loop]
	while (true)
	{
		uint stack = WaveActiveMin(pending);
		if (stack == NO_STACK)
		{
			break;
		}

		bool mine = pending == stack;
		uint total = WaveActiveCountBits(mine);

		if (WaveIsFirstLane())
		{
			uint ignored;
			InterlockedAdd(BinCounts[stack], total, ignored);
		}

		pending = mine ? NO_STACK : pending;
	}
}
