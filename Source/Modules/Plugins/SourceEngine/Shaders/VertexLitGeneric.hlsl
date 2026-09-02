
void SFMain(inout SurfaceModel surface, in SFInput input)
{
	float4 baseTex = input.BaseTexture.SampleGrad(DefaultSampler, input.UV0, input.DDX, input.DDY);
	float4 bumpTex = input.BumpMap.SampleGrad(DefaultSampler, input.UV0, input.DDX, input.DDY);

	surface.Albedo = LinearToSRGB(baseTex.rgb) * input.Color.rgb;
	surface.Normal = bumpTex.rgb * 2 - 1;
	surface.Emissive = surface.Albedo * input.SelfIllumTint.rgb * 33;

	// $phong masks are dark intensity maps that $phongboost compensates for, so rescale to a gate.
	float rawMask = input.PhongMaskSource == 1 ? baseTex.a : bumpTex.a;
	float mask = input.PhongMaskSource == 0 ? 1 : smoothstep(0.05, 0.45, rawMask);

	// A $phongexponenttexture overrides the scalar exponent per texel.
	float exponentTex = input.ExponentTexture.SampleGrad(DefaultSampler, input.UV0, input.DDX, input.DDY).r;
	float exponent = exponentTex > 0 ? exponentTex * 149 + 1 : input.PhongExponent;

	// Sheen tightness scaled by its strength, offset so even unmasked cloth reads matte rather than flat.
	float gloss = exponent > 0 ? log2(max(exponent, 1)) * sqrt(input.PhongBoost) / 8 : 0;
	surface.Roughness = max(lerp(1, 1 - gloss, mask) - 0.21, 0.2);
}
