#include "Shaders/Common.h"
#include "Shaders/Lighting.h"
#include "Shaders/Raytracing.h"

// Bit i is light i's visibility, so lighting resolves every light from one fetch.
#define MAX_SHADOWED_LIGHTS 32

RWTexture2D<uint> ShadowMask : register(u0);

Texture2D<half4> MatBuffer1 : register(t0);
Texture2D<float> DepthBuffer : register(t1);

cbuffer Constants : register(b0)
{
	int LightCount;
}

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

		int count = min(LightCount, MAX_SHADOWED_LIGHTS);
		for (int i = 0; i < count; i++)
		{
			Light light = Lights[i];

			float3 delta = light.Position - origin;
			float dist = length(delta);

			if (light.Type == LIGHT_NONE || dist > light.Range || dot(normal, delta) <= 0)
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
