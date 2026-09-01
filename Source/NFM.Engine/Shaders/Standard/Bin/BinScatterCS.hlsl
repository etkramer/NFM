#include "Shaders/World.h"

Texture2D<uint2> VisBuffer : register(t0);
Texture2D<float> DepthBuffer : register(t1);

ByteAddressBuffer MaterialParams : register(t0, space2);
StructuredBuffer<uint> BinOffsets : register(t2);

RWStructuredBuffer<uint> BinCursors : register(u0);
RWStructuredBuffer<uint> BinPixels : register(u1);

[numthreads(32, 32, 1)]
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

	uint slot;
	InterlockedAdd(BinCursors[shaderID], 1, slot);

	BinPixels[BinOffsets[shaderID] + slot] = (id.x & 0xFFFF) | (id.y << 16);
}
