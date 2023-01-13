#include "Content/Include/Samplers.h"

Texture2D<float4> Source : register(t0);
RWTexture2D<float4> Dest : register(u0);

cbuffer Constants : register(b0)
{
	float2 TexelSize;
}

[numthreads(8, 8, 1)]
void MipGenCS(uint3 pixel : SV_DispatchThreadID)
{
	float2 texCoords = TexelSize * (pixel.xy + 0.5);

	// Sample texel and write to output.
	Dest[pixel.xy] = Source.SampleLevel(LinearSampler, texCoords, 0);
}