#include "Shaders/Common.h"
#include "Shaders/World.h"

RWTexture2D<float4> RT : register(u0);

Texture2D<float4> MatBuffer0 : register(t0);
Texture2D<half4> MatBuffer1 : register(t1);
Texture2D<float4> MatBuffer2 : register(t2);
Texture2D<float> DepthBuffer : register(t3);

[numthreads(32, 32, 1)]
void main(uint2 id : SV_DispatchThreadID)
{
	// Grab the frame width/height
	int2 frameSize;
	RT.GetDimensions(frameSize.x, frameSize.y);

	// Don't try to process out of bounds pixels
	if (id.x >= frameSize.x || id.y >= frameSize.y)
	{
		return;
	}

	// Early out if this pixel is empty
	if (DepthBuffer[id] == 0)
	{
		return;
	}
	
	// Unpack g-buffer
	float3 albedo = MatBuffer0[id].rgb;
	half3 normal = MatBuffer1[id].rgb;
	float3 msr = MatBuffer2[id].rgb;
	
	RT[id] = SRGBToLinear(float4(albedo, 1));
}