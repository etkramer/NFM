#pragma once

#define SHADING_LIT 0
#define SHADING_UNLIT 1 // Emits its own radiance, and takes no response from lights

struct SurfaceModel
{
	// Geometry (tangent space)
	float3 Normal;

	// PBR
	float3 Albedo;
	float Metallic;
	float Roughness;
	float Specular;
	float3 Emissive;

	// Non-opaque
	float Opacity;

	// SHADING_ constant, selecting how the lighting passes treat the surface
	uint ShadingModel;
};
