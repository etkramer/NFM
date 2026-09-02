#include "Shaders/Standard/Forward/Forward.h"
#include "Shaders/SurfaceModel.h"
#include "Shaders/Shading.h"

// Linked in from the material's shader permutation.
SurfaceModel EvalSurface(uint materialID, float2 uv0, float2 ddx, float2 ddy);

// Named pixel entry point, for library compilation.
[shader("pixel")]
float4 main(ForwardVertex input, uint isFrontFace : SV_IsFrontFace) : SV_Target0
{
	Instance instance = Instances[InstanceID];

	// Real pixel-quad derivatives, unlike the visbuffer's analytic reconstruction.
	SurfaceModel surface = EvalSurface(instance.MaterialID, input.UV0, ddx(input.UV0), ddy(input.UV0));

	if (surface.ShadingModel == SHADING_UNLIT)
	{
		return float4(surface.Emissive, surface.Opacity);
	}

	// Two-sided geometry shades its back faces as if they pointed at the viewer.
	float facing = isFrontFace ? 1 : -1;

	float3 normal = normalize(input.Normal) * facing;
	float3 tangent = normalize(input.Tangent.xyz - normal * dot(normal, input.Tangent.xyz));
	float3 bitangent = cross(normal, tangent) * input.Tangent.w * facing;
	float3x3 tangentToWorld = float3x3(tangent, bitangent, normal);

	Surface shaded;
	shaded.Position = input.WorldPos;
	shaded.Normal = normalize(mul(surface.Normal, tangentToWorld));
	shaded.Albedo = surface.Albedo;
	shaded.Metallic = surface.Metallic;
	shaded.Specular = surface.Specular;
	shaded.Roughness = surface.Roughness;

	float3 V = normalize(ViewConstants.EyePosition - shaded.Position);

	// The shadow mask covers opaque depth only, so it can't be sampled here.
	float3 color = ShadeSurface(shaded, V, surface.Emissive, uint2(input.Position.xy), SHADOW_ALL);

	return float4(color, surface.Opacity);
}
