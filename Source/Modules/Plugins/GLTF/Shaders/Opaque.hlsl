
void SFMain(inout SurfaceModel surface, in SFInput input)
{
	float4 color = input.BaseColor.SampleGrad(DefaultSampler, input.UV0, input.DDX, input.DDY);
	float4 orm = input.ORM.SampleGrad(DefaultSampler, input.UV0, input.DDX, input.DDY);
	float4 normal = input.Normal.SampleGrad(DefaultSampler, input.UV0, input.DDX, input.DDY);
	
	// No way to know if a texture is sRGB during import,
	// so we just convert it in the shader.
	surface.Albedo = LinearToSRGB(color.rgb);
	surface.Normal = normal.rgb;
	surface.Roughness = orm[1];
	surface.Metallic = orm[2];
}