
void SFMain(inout SurfaceModel surface, in SFInput input)
{
	float4 baseTex = input.BaseTexture.SampleGrad(DefaultSampler, input.UV0, input.DDX, input.DDY);

	// Base colour is emitted outright; alpha is linear coverage, and stays undecoded.
	surface.ShadingModel = SHADING_UNLIT;
	surface.Emissive = LinearToSRGB(baseTex.rgb) * input.Color.rgb * 33;
	surface.Opacity = baseTex.a * input.Color.a;
}
