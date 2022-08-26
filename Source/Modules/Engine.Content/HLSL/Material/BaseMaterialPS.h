struct SurfaceInfo
{
	// Basic
	float3 Color;
	float3 Normal;
	float Metallic;
	float Roughness;
	float Emission;

	// Non-opaque
	float Opacity;
};