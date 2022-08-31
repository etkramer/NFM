// Consult https://google.github.io/filament/images/material_chart.jpg
struct Surface
{
	// Geoemtry
	float3 Normal;

	// PBR
	float3 Albedo;
	float Metallic;
	float Roughness;
	float Reflectance;

	// Non-opaque
	float Opacity;
};

Surface GetDefaultSurface()
{
	Surface defaults;
	defaults.Albedo = float3(0, 0, 0);
	defaults.Normal = float3(0.5, 0.5, 1);
	defaults.Metallic = 0;
	defaults.Roughness = 0.5;
	defaults.Reflectance = 0.5; // 4%
	defaults.Opacity = 1;

	return defaults;
}