#include "Shaders/Common.h"

RWTexture2D<float4> RT : register(u0);

Texture2D<float4> SceneColor : register(t0);

cbuffer Constants : register(b0)
{
	int ApplyTonemap;
	float Exposure;
}

// Rotates into the AgX working space, where the sigmoid is applied.
static const float3x3 AgXTransform = float3x3(
	0.842479062253094, 0.0784335999999992, 0.0792237451477643,
	0.0423282422610123, 0.878468636469772, 0.0791661274605434,
	0.0423756549057051, 0.0784336000000000, 0.879142973793104);

static const float3x3 AgXTransformInverse = float3x3(
	 1.19687900512017,  -0.0980208811401368, -0.0990297440797205,
	-0.0528968517574562, 1.15190312990417,   -0.0989611768448433,
	-0.0529716355144438, -0.0980434501171241,  1.15107367264116);

// Wrensch's polynomial fit of the AgX contrast sigmoid.
float3 AgXContrast(float3 x)
{
	float3 x2 = x * x;
	float3 x4 = x2 * x2;

	return 15.5 * x4 * x2
		- 40.14 * x4 * x
		+ 31.96 * x4
		- 6.868 * x2 * x
		+ 0.4298 * x2
		+ 0.1191 * x
		- 0.00232;
}

// Maps open-domain radiance to a display-encoded value. Output is already gamma
// encoded, so it needs no further sRGB transfer.
float3 AgX(float3 color)
{
	const float minEV = -12.47393;
	const float maxEV = 4.026069;

	color = mul(AgXTransform, color);

	// Log encode across the supported exposure range, then apply the sigmoid.
	color = clamp(log2(color), minEV, maxEV);
	color = (color - minEV) / (maxEV - minEV);
	color = AgXContrast(color);

	return saturate(mul(AgXTransformInverse, color));
}

[numthreads(8, 8, 1)]
void main(uint2 id : SV_DispatchThreadID)
{
	int2 frameSize;
	RT.GetDimensions(frameSize.x, frameSize.y);

	if (id.x >= frameSize.x || id.y >= frameSize.y)
	{
		return;
	}

	float3 color = SceneColor[id].rgb;

	if (ApplyTonemap)
	{
		color = AgX(color * Exposure);
	}
	else
	{
		// Debug views carry data rather than radiance, so they only need the transfer function.
		color = SRGBToLinear(color);
	}

	RT[id] = float4(color, 1);
}
