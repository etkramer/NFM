#include "Shaders/World.h"

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
	uint shaderID = MaterialParams.Load(materialID);

	// Each wave claims one contiguous run of slots per distinct stack; lanes index it by prefix count.
	while (true)
	{
		uint stack = WaveReadLaneFirst(shaderID);
		if (stack == shaderID)
		{
			uint laneSlot = WavePrefixCountBits(true);
			uint total = WaveActiveCountBits(true);

			uint runStart = 0;
			if (WaveIsFirstLane())
			{
				InterlockedAdd(BinCursors[stack], total, runStart);
			}

			BinPixels[BinOffsets[stack] + WaveReadLaneFirst(runStart) + laneSlot] = (id.x & 0xFFFF) | (id.y << 16);
			break;
		}
	}
}
