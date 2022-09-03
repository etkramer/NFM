#include "HLSL/Include/Common.h"

RWTexture2D<float4> RenderTarget : register(u0);

[numthreads(32, 32, 1)]
void GammaCorrectCS(uint3 dispatchID : SV_DispatchThreadID)
{
	int width, height;
	RenderTarget.GetDimensions(width, height);

	// Don't try to process out of bounds pixels.
	if (dispatchID.x > width || dispatchID.y > height)
	{
		return;
	}

	// Apply gamma correction.
	RenderTarget[dispatchID.xy] = float4(SRGBToLinear(RenderTarget[dispatchID.xy].xyz), 1);
}