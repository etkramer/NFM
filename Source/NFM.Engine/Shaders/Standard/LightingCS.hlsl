#include "Shaders/Common.h"
#include "Shaders/Lighting.h"

RWTexture2D<float4> RT : register(u0);

Texture2D<float4> MatBuffer0 : register(t0);
Texture2D<half4> MatBuffer1 : register(t1);
Texture2D<float4> MatBuffer2 : register(t2);
Texture2D<float4> MatBuffer3 : register(t3);
Texture2D<float> DepthBuffer : register(t4);

cbuffer Constants : register(b0)
{
	int DisplayMode;
	int LightCount;
}

#define DISPLAY_LIT 0
#define DISPLAY_UNLIT 1
#define DISPLAY_NORMALS 2
#define DISPLAY_METALLIC 3
#define DISPLAY_SPECULAR 4
#define DISPLAY_ROUGHNESS 5

float3 ReconstructWorldPosition(uint2 id, int2 frameSize, float depth)
{
	float2 ndc = ((float2(id) + 0.5) / float2(frameSize)) * 2 - 1;
	ndc.y *= -1;

	float4 viewPos = mul(ViewConstants.ClipToView, float4(ndc, depth, 1));
	return mul(ViewConstants.ViewToWorld, float4(viewPos.xyz / viewPos.w, 1)).xyz;
}

[numthreads(8, 8, 1)]
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

	// Output background color for empty pixels
	float depth = DepthBuffer[id];
	if (depth == 0)
	{
		RT[id] = float4(0.1, 0.1, 0.1, 1);
		return;
	}

	// Unpack g-buffer
	float3 albedo = MatBuffer0[id].rgb;
	float3 normal = MatBuffer1[id].rgb;
	float3 msr = MatBuffer2[id].rgb;

	float3 color = albedo;
	switch (DisplayMode)
	{
		case DISPLAY_NORMALS: color = normal * 0.5 + 0.5; break;
		case DISPLAY_METALLIC: color = msr.r; break;
		case DISPLAY_SPECULAR: color = msr.g; break;
		case DISPLAY_ROUGHNESS: color = msr.b; break;
		case DISPLAY_LIT:
		{
			Surface surface;
			surface.Position = ReconstructWorldPosition(id, frameSize, depth);
			surface.Normal = normalize(normal);
			surface.Albedo = albedo;
			surface.Metallic = msr.r;
			surface.Specular = msr.g;
			surface.Roughness = msr.b;

			float3 V = normalize(ViewConstants.EyePosition - surface.Position);

			color = MatBuffer3[id].rgb;
			for (int i = 0; i < LightCount; i++)
			{
				color += EvalLight(surface, V, Lights[i]);
			}

			break;
		}
	}

	RT[id] = float4(color, 1);
}
