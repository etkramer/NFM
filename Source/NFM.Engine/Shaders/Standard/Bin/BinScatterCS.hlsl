#include "Shaders/World.h"

#define NO_STACK 0xFFFFFFFF

Texture2D<uint2> VisBuffer : register(t0);
Texture2D<float> DepthBuffer : register(t1);

ByteAddressBuffer MaterialParams : register(t0, space2);
StructuredBuffer<uint> BinOffsets : register(t2);

RWStructuredBuffer<uint> BinCursors : register(u0);
RWStructuredBuffer<uint> BinPixels : register(u1);

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
	uint packed = (id.x & 0xFFFF) | (id.y << 16);

	// Each wave claims one contiguous run of slots per distinct stack; lanes index it by prefix count.
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
		uint laneSlot = WavePrefixCountBits(mine);

		uint runStart = 0;
		if (WaveIsFirstLane())
		{
			InterlockedAdd(BinCursors[stack], total, runStart);
		}

		runStart = WaveReadLaneFirst(runStart);

		if (mine)
		{
			BinPixels[BinOffsets[stack] + runStart + laneSlot] = packed;
			pending = NO_STACK;
		}
	}
}
