static Texture2D<float3> BaseColor;
static Texture2D<float3> Normal;
static Texture2D<float3> ORM;

Surface SurfaceMain(in SurfaceState state)
{
	Surface surface = state.Defaults;
	
	float3 orm = ORM.Sample(DefaultSampler, state.UV);
	
	// No way to know if a texture is sRGB during import,
	// so we just convert it in the shader.
	surface.Albedo = LinearToSRGB(BaseColor.Sample(DefaultSampler, state.UV));
	surface.Normal = Normal.Sample(DefaultSampler, state.UV);
	surface.Roughness = orm[1];
	surface.Metallic = orm[2];
	
	return surface;
}