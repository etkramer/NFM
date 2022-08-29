#include "HLSL/Include/Common.h"
#include "HLSL/Include/Geometry.h"

Texture2D<float> DepthBuffer : register(t0);
Texture2D<float4> MatBuffer0 : register(t1); // RGB: Color, A: Emission
Texture2D<float4> MatBuffer1 : register(t2); // RG: Normal, B: Roughness, A: Metallic

RWTexture2D<float4> Output : register(u0);

[NumThreads(32, 32, 1)]
void LightingCS(uint3 pixel : SV_DispatchThreadID)
{
	// Get the RT dimensions.
	uint width, height;
	Output.GetDimensions(width, height);

	// Return if this pixel is out of bounds.
	if (pixel.x > width || pixel.y > height)
	{
		return;
	}

	// Sample depth.
	float2 texCoords = pixel / (float)width;
	float depth = DepthBuffer.Load(pixel);

	// Return if this pixel does not contain any geometry.
	if (depth == 0)
	{
		return;
	}

	// Sample material (G-) buffers.
	float4 mat0 = MatBuffer0.Load(pixel);
	float4 mat1 = MatBuffer1.Load(pixel);

	// Reconstruct normal Z component.
	float3 norm = float3(mat1.xy * 2 - 1, 0);
	norm.z = sqrt(1 - dot(norm.xy, norm.xy));

	Output[pixel.xy] = float4(norm, 1);
	//Output[pixel.xy] = float4(mat0.xyz, 1);
}