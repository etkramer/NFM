#include "Shaders/Include/Geometry.hlsl"

Texture2D<int> VisBuffer : register(t0);
Texture2D<float> DepthBuffer : register(t1);

RWTexture2D<float4> RenderTarget : register(u0);

[numthreads(32, 32, 1)]
void ComputeEntry(uint3 ID : SV_DispatchThreadID)
{
	// Early out if this pixel is empty.
	bool isEmpty = DepthBuffer[pixel] == 0;
	if (isEmpty)
	{
		return;
	}

	// Calculate output color.
	float4 output = float4(1, 0, 1, 1);

	// Write it to the render target.
	RenderTarget[ID.xy] = output;
}