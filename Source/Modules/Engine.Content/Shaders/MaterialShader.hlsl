#include "Shaders/Include/Geometry.hlsl"

//Texture2D<int> VisBuffer : register(t0);
Texture2D<float> DepthBuffer : register(t1);
RWTexture2D<float4> RenderTarget : register(u0);

bool IsInsideBounds(uint3 dispatchThreadID)
{
	float width, height;
	RenderTarget.GetDimensions(width, height);

	return !(dispatchThreadID.x >= width || dispatchThreadID.y >= height);
}

[numthreads(32, 32, 1)]
void ComputeEntry(uint3 ID : SV_DispatchThreadID)
{
	// Make sure we don't try to read outside the bounds of the RT.
	if (!IsInsideBounds(ID))
	{
		return;
	}

	// Early out if this pixel is empty.
	bool isEmpty = DepthBuffer[ID.xy] == 0;
	if (isEmpty)
	{
		return;
	}

	// Calculate output color.
	float4 output = float4(1, 0, 1, 1);

	// Write it to the render target.
	RenderTarget[ID.xy] = output;
}