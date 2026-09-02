#include "Shaders/Common.h"
#include "Shaders/Lighting.h"
#include "Shaders/Clustering.h"

RWTexture2D<float4> RT : register(u0);

Texture2D<float4> MatBuffer0 : register(t0);
Texture2D<half4> MatBuffer1 : register(t1);
Texture2D<float4> MatBuffer2 : register(t2);
Texture2D<float4> MatBuffer3 : register(t3);
Texture2D<float> DepthBuffer : register(t4);
Texture2D<uint> ShadowMask : register(t5);

StructuredBuffer<uint> ClusterCounts : register(t9, space1);
StructuredBuffer<uint> ClusterOffsets : register(t10, space1);
StructuredBuffer<uint> ClusterLights : register(t11, space1);

cbuffer Constants : register(b0)
{
	int DisplayMode;
}

#define DISPLAY_LIT 0
#define DISPLAY_UNLIT 1
#define DISPLAY_NORMALS 2
#define DISPLAY_METALLIC 3
#define DISPLAY_SPECULAR 4
#define DISPLAY_ROUGHNESS 5
#define DISPLAY_SHADOWS 6
#define DISPLAY_CLUSTERS 7

bool IsLightVisible(uint shadowMask, uint index)
{
	return index >= MAX_SHADOWED_LIGHTS || (shadowMask & (1u << index)) != 0;
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

	float3 position = ReconstructWorldPosition(id, frameSize, depth);

	uint shadowMask = ShadowMask[id];
	uint cluster = ClusterFromPixel(id, position);
	uint offset = ClusterOffsets[cluster];
	uint count = ClusterLightCount(offset, ClusterCounts[cluster]);

	float3 color = albedo;
	switch (DisplayMode)
	{
		case DISPLAY_NORMALS: color = normal * 0.5 + 0.5; break;
		case DISPLAY_METALLIC: color = msr.r; break;
		case DISPLAY_SPECULAR: color = msr.g; break;
		case DISPLAY_ROUGHNESS: color = msr.b; break;
		case DISPLAY_CLUSTERS: color = count == 0 ? 0 : ColorFromIndex(count); break;
		case DISPLAY_SHADOWS: color = countbits(shadowMask) / max(float(min(count, MAX_SHADOWED_LIGHTS)), 1); break;
		case DISPLAY_LIT:
		{
			Surface surface;
			surface.Position = position;
			surface.Normal = normalize(normal);
			surface.Albedo = albedo;
			surface.Metallic = msr.r;
			surface.Specular = msr.g;
			surface.Roughness = msr.b;

			float3 V = normalize(ViewConstants.EyePosition - surface.Position);

			color = MatBuffer3[id].rgb;
			for (uint i = 0; i < count; i++)
			{
				if (IsLightVisible(shadowMask, i))
				{
					color += EvalLight(surface, V, Lights[ClusterLights[offset + i]]);
				}
			}

			break;
		}
	}

	RT[id] = float4(color, 1);
}
