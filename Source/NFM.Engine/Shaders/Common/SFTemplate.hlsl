#include "Shaders/Common.h"

struct SurfaceModel
{
	// Geoemtry
	float3 Normal;

	// PBR
	float3 Albedo;
	float Metallic;
	float Roughness;
	float Specular;

	// Non-opaque
	float Opacity;
};

struct SFInput
{
	float2 UV0;
	
	float2 DDX;
	float2 DDY;
	
	#pragma PARAMS
};

void SFMain(inout SurfaceModel surface, in SFInput input);

#pragma MAIN

ByteAddressBuffer MaterialParams : register(t0);

export SurfaceModel EvalSurface(uint materialID, float2 uv0, float2 ddx, float2 ddy)
{
	// Setup defaults
	SurfaceModel model;
	model.Albedo = float3(0, 0, 0);
	model.Normal = float3(0.5, 0.5, 1);
	model.Metallic = 0;
	model.Roughness = 0.5;
	model.Specular = 0.5;
	model.Opacity = 1;
	
	// Read material params from buffer.
	uint shaderID = MaterialParams.Load(materialID + 0);
	
	// Create inputs structure
	SFInput input;
	input.UV0 = uv0;
	
	#pragma SETUP
	
	// Invoke surface shader
	SFMain(model, input);
	return model;
}