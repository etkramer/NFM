#include "Shaders/Common.h"

RWTexture2D<float4> RT : register(u0);
Texture2D<uint2> VisBuffer : register(t0);
Texture2D<float> DepthBuffer : register(t1);

[numthreads(32, 32, 1)]
void PreviewCS(uint3 pixel : SV_DispatchThreadID)
{
	// Don't try to process out of bounds pixels.
	int width, height;
	RT.GetDimensions(width, height);
	if (pixel.x >= width || pixel.y >= height)
	{
		return;
	}

	if (DepthBuffer[pixel.xy] == 0)
	{
		return;
	}

	uint2 packed = VisBuffer[pixel.xy];
	uint instanceID = packed.x;
	uint triangleID = packed.y;

	RT[pixel.xy] = float4(ColorFromIndex(instanceID), 1);
}