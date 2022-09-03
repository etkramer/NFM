static Texture2D<float3> BaseColor;
static Texture2D<float3> Normal;

Surface SurfaceMain(in SurfaceState state)
{
	Surface surface = state.Defaults;
	
	// No way to know if a texture is sRGB during import,
	// so we just convert it in the shader.
	surface.Albedo = LinearToSRGB(BaseColor.Sample(DefaultSampler, state.UV));
	surface.Normal = Normal.Sample(DefaultSampler, state.UV);
	
	return surface;
}