static Texture2D<float3> BaseColor;
static Texture2D<float3> Normal;

Surface SurfaceMain(in SurfaceState state)
{
	Surface surface = state.Defaults;
	
	surface.Albedo = BaseColor.Sample(DefaultSampler, state.UV);
	surface.Normal = Normal.Sample(DefaultSampler, state.UV);
	
	return surface;
}