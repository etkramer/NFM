#pragma once

#include "Lighting.h"
#include "Clustering.h"

// The cluster light lists, as built by the cluster passes.
StructuredBuffer<uint> ClusterCounts : register(t9, space1);
StructuredBuffer<uint> ClusterOffsets : register(t10, space1);
StructuredBuffer<uint> ClusterLights : register(t11, space1);

// Every light shaded, for paths with no screen-space shadow mask to sample.
#define SHADOW_ALL 0xFFFFFFFF

bool IsLightVisible(uint shadowMask, uint index)
{
	return index >= MAX_SHADOWED_LIGHTS || (shadowMask & (1u << index)) != 0;
}

// Emissive plus every light touching the surface's cluster.
float3 ShadeSurface(Surface surface, float3 V, float3 emissive, uint2 pixel, uint shadowMask)
{
	uint cluster = ClusterFromPixel(pixel, surface.Position);
	uint offset = ClusterOffsets[cluster];
	uint count = ClusterLightCount(offset, ClusterCounts[cluster]);

	float3 color = emissive;
	for (uint i = 0; i < count; i++)
	{
		if (IsLightVisible(shadowMask, i))
		{
			color += EvalLight(surface, V, Lights[ClusterLights[offset + i]]);
		}
	}

	return color;
}
