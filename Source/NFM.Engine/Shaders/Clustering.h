#pragma once

#include "World.h"
#include "Lighting.h"

#define CLUSTER_TILE_SIZE 16
#define CLUSTER_SLICES 32

// Shared by every cluster; a scene dense enough to exhaust it drops its last few assignments.
#define CLUSTER_LIGHT_POOL (4 * 1024 * 1024)

// Lights past this many in a cluster go unshadowed, being beyond what one mask can hold.
#define MAX_SHADOWED_LIGHTS 32

// Slices are spaced exponentially over distance from the eye, so each tracks a constant projected size.
uint ClusterSlice(float depth)
{
	float slice = log2(max(depth, 1e-4)) * ViewConstants.ClusterScale + ViewConstants.ClusterBias;
	return (uint)clamp(slice, 0, CLUSTER_SLICES - 1);
}

uint ClusterIndex(uint3 coord)
{
	return (coord.z * ViewConstants.ClusterDims.y + coord.y) * ViewConstants.ClusterDims.x + coord.x;
}

// The cluster a shaded pixel falls in.
uint ClusterFromPixel(uint2 pixel, float3 position)
{
	uint slice = ClusterSlice(distance(ViewConstants.EyePosition, position));
	return ClusterIndex(uint3(pixel / CLUSTER_TILE_SIZE, slice));
}

// Assignments past the pool's end were never written, so they're dropped rather than read back.
uint ClusterLightCount(uint offset, uint count)
{
	return offset < CLUSTER_LIGHT_POOL ? min(count, CLUSTER_LIGHT_POOL - offset) : 0;
}

// Conservative range of clusters a light's sphere of influence can touch, inclusive on both ends.
bool LightClusterBounds(Light light, int2 frameSize, out uint3 lo, out uint3 hi)
{
	lo = 0;
	hi = 0;

	float range = sqrt(LightRangeSq(light));
	if (light.Type == LIGHT_NONE || range <= 0)
	{
		return false;
	}

	float depth = distance(ViewConstants.EyePosition, light.Position);
	lo.z = ClusterSlice(max(depth - range, 0));
	hi.z = ClusterSlice(depth + range);

	float2 uvMin = 0;
	float2 uvMax = 1;

	// A sphere reaching the eye projects to nothing usable, so it just claims the whole screen.
	if (depth > range)
	{
		float2 boundsMin = 1e30;
		float2 boundsMax = -1e30;
		bool straddles = false;

		for (int i = 0; i < 8; i++)
		{
			float3 corner = light.Position + (float3(i & 1, (i >> 1) & 1, (i >> 2) & 1) * 2 - 1) * range;
			float4 clip = mul(ViewConstants.ViewToClip, mul(ViewConstants.WorldToView, float4(corner, 1)));

			straddles = straddles || clip.w <= 0;

			float2 uv = float2(clip.x, -clip.y) / max(clip.w, 1e-6) * 0.5 + 0.5;
			boundsMin = min(boundsMin, uv);
			boundsMax = max(boundsMax, uv);
		}

		if (!straddles)
		{
			if (any(boundsMax < 0) || any(boundsMin > 1))
			{
				return false;
			}

			uvMin = saturate(boundsMin);
			uvMax = saturate(boundsMax);
		}
	}

	uint2 lastTile = ViewConstants.ClusterDims.xy - 1;
	lo.xy = min((uint2)(uvMin * frameSize) / CLUSTER_TILE_SIZE, lastTile);
	hi.xy = min((uint2)(uvMax * frameSize) / CLUSTER_TILE_SIZE, lastTile);

	return true;
}
