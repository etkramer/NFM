#include "Shaders/Common.h"
#include "Shaders/Lighting.h"
#include "Shaders/Raytracing.h"
#include "Shaders/Clustering.h"

RWTexture2D<uint> ShadowMask : register(u0);

Texture2D<half4> MatBuffer1 : register(t0);
Texture2D<float> DepthBuffer : register(t1);

StructuredBuffer<uint> ClusterCounts : register(t9, space1);
StructuredBuffer<uint> ClusterOffsets : register(t10, space1);
StructuredBuffer<uint> ClusterLights : register(t11, space1);

[numthreads(8, 8, 1)]
void main(uint2 id : SV_DispatchThreadID)
{
	int2 frameSize;
	ShadowMask.GetDimensions(frameSize.x, frameSize.y);

	if (id.x >= frameSize.x || id.y >= frameSize.y)
	{
		return;
	}

	uint mask = 0;
	float depth = DepthBuffer[id];

	if (depth != 0)
	{
		float3 position = ReconstructWorldPosition(id, frameSize, depth);
		float3 normal = normalize(MatBuffer1[id].rgb);

		// Lift the origin off the surface by more the coarser this pixel is in world units.
		float3 origin = position + normal * (0.001 + 0.0005 * distance(ViewConstants.EyePosition, position));

		uint cluster = ClusterFromPixel(id, position);
		uint offset = ClusterOffsets[cluster];
		uint count = min(ClusterLightCount(offset, ClusterCounts[cluster]), MAX_SHADOWED_LIGHTS);

		// Bit i tracks the cluster's i'th light, so lighting resolves the same walk from one fetch.
		for (uint i = 0; i < count; i++)
		{
			Light light = Lights[ClusterLights[offset + i]];

			float3 delta = light.Position - origin;
			float dist = length(delta);

			if (light.Type == LIGHT_NONE || dist * dist > LightRangeSq(light) || dot(normal, delta) <= 0)
			{
				continue;
			}

			// Stop short of the source, so geometry enclosing the light doesn't occlude it.
			if (TraceVisibility(origin, delta / dist, max(dist - light.Radius, 0)))
			{
				mask |= 1u << i;
			}
		}
	}

	ShadowMask[id] = mask;
}
