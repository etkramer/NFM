#include "Shaders/World.h"

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
	uint shaderID = MaterialParams.Load(materialID);

	// Each wave contributes a single atomic per distinct stack it covers.
	while (true)
	{
		uint stack = WaveReadLaneFirst(shaderID);
		if (stack == shaderID)
		{
			uint total = WaveActiveCountBits(true);
			if (WaveIsFirstLane())
			{
				uint ignored;
				InterlockedAdd(BinCounts[stack], total, ignored);
			}

			break;
		}
	}
}
