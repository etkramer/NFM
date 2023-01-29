#include "Shaders/Common.h"
#include "Shaders/World.h"

RWTexture2D<float4> RT : register(u0);

Texture2D<float4> MatBuffer0 : register(t0);
Texture2D<half2> MatBuffer1 : register(t1);
Texture2D<float4> MatBuffer2 : register(t2);
Texture2D<float> DepthBuffer : register(t3);

// https://github.com/DigitalRune/DigitalRune/blob/master/Source/DigitalRune.Graphics.Content/DigitalRune/Encoding.fxh
half3 DecodeNormalSphereMap(half4 encodedNormal)
{
	half4 nn = encodedNormal * half4(2, 2, 0, 0) + half4(-1, -1, 1, -1);
	half l = dot(nn.xyz, -nn.xyw);
	nn.z = l;
	nn.xy *= (half) sqrt(l);
	return nn.xyz * 2 + half3(0, 0, -1);
}

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
	half3 normal = DecodeNormalSphereMap(float4(MatBuffer1[id], 0, 0));
	float3 msr = MatBuffer2[id].rgb;
	
	RT[id] = SRGBToLinear(float4(albedo, 1));
}