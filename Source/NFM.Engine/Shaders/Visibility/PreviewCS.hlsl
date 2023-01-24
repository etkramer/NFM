#include "Shaders/Common.h"

RWTexture2D<float4> RT : register(u0);
Texture2D<uint2> VisBuffer : register(t0);
Texture2D<float> DepthBuffer : register(t1);

[numthreads(32, 32, 1)]
void PreviewCS(uint3 pixel : SV_DispatchThreadID)
{
	int width, height;
	RT.GetDimensions(width, height);

	// Don't try to process out of bounds pixels.
	if (pixel.x >= width || pixel.y >= height)
	{
		return;
	}

	if (DepthBuffer[pixel.xy] == 0)
	{
		return;
	}

	uint2 value = VisBuffer[pixel.xy];
	uint2 unpacked = UnpackBits(value.x, 25);

	uint meshletID = unpacked.x;
	uint primID = unpacked.y;
	uint instanceID = value.y;

	RT[pixel.xy] = float4(ColorFromIndex(primID), 1);
}