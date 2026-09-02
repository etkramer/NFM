#pragma once

#include "World.h"

#define PI 3.14159265359

#define LIGHT_NONE 0
#define LIGHT_POINT 1

// Nothing is binned past the cluster grid's far distance, so no light reaches further.
#define LIGHT_MAX_RANGE 512

// Distance squared at which a light stops contributing more than the display can resolve. Derived
// from the view's own exposure, so ranges track it without anything to author.
float LightRangeSq(Light light)
{
	float peak = max(light.Color.r, max(light.Color.g, light.Color.b));
	return min(peak * ViewConstants.InvLightCutoff, LIGHT_MAX_RANGE * LIGHT_MAX_RANGE);
}

// A shaded point, as unpacked from the g-buffer or hit by a ray.
struct Surface
{
	float3 Position;
	float3 Normal;

	float3 Albedo;
	float Metallic;
	float Specular;
	float Roughness;
};

float3 F_Schlick(float3 f0, float vdoth)
{
	return f0 + (1 - f0) * pow(1 - vdoth, 5);
}

// Trowbridge-Reitz, with a = roughness squared.
float D_GGX(float ndoth, float a)
{
	float a2 = a * a;
	float d = (ndoth * ndoth) * (a2 - 1) + 1;
	return a2 / (PI * d * d);
}

// Height-correlated Smith, with the 1 / (4 * ndotl * ndotv) of the BRDF folded in.
float V_SmithGGX(float ndotv, float ndotl, float a)
{
	float a2 = a * a;
	float lambdaV = ndotl * sqrt((ndotv - ndotv * a2) * ndotv + a2);
	float lambdaL = ndotv * sqrt((ndotl - ndotl * a2) * ndotl + a2);
	return 0.5 / max(lambdaV + lambdaL, 1e-5);
}

// Lambert diffuse plus GGX specular, without the NdotL cosine term.
float3 EvalBRDF(Surface surface, float3 V, float3 L)
{
	float3 N = surface.Normal;
	float3 H = normalize(V + L);

	float ndotv = abs(dot(N, V)) + 1e-5;
	float ndotl = saturate(dot(N, L));
	float ndoth = saturate(dot(N, H));
	float vdoth = saturate(dot(V, H));

	// Dielectrics reflect a fixed fraction, metals reflect their own albedo.
	float3 f0 = lerp(0.08 * surface.Specular, surface.Albedo, surface.Metallic);
	float3 diffuse = surface.Albedo * (1 - surface.Metallic) / PI;

	float a = surface.Roughness * surface.Roughness;
	float3 specular = D_GGX(ndoth, a) * V_SmithGGX(ndotv, ndotl, a) * F_Schlick(f0, vdoth);

	return diffuse + specular;
}

// Radiance reaching the surface from one light, ignoring occlusion.
float3 EvalLight(Surface surface, float3 V, Light light)
{
	if (light.Type == LIGHT_NONE)
	{
		return 0;
	}

	float3 delta = light.Position - surface.Position;
	float distSq = dot(delta, delta);
	float rangeSq = LightRangeSq(light);

	if (distSq > rangeSq)
	{
		return 0;
	}

	float3 L = delta * rsqrt(max(distSq, 1e-8));
	float ndotl = saturate(dot(surface.Normal, L));

	// Clamped inside the source radius, where inverse-square would blow up.
	float attenuation = rcp(max(distSq, light.Radius * light.Radius));

	// Eased to nothing by the range, so the cutoff never shows up as an edge.
	float ratio = distSq / rangeSq;
	float window = saturate(1 - ratio * ratio);

	return EvalBRDF(surface, V, L) * light.Color * attenuation * window * window * ndotl;
}
