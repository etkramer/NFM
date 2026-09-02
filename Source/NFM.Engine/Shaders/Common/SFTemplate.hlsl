#include "Shaders/Common.h"
#include "Shaders/SurfaceModel.h"

struct SFInput
{
	float2 UV0;
	
	float2 DDX;
	float2 DDY;
	
	#pragma PARAMS
};

void SFMain(inout SurfaceModel surface, in SFInput input);

#pragma MAIN

ByteAddressBuffer MaterialParams : register(t0, space2);

export SurfaceModel EvalSurface(uint materialID, float2 uv0, float2 ddx, float2 ddy)
{
	// Setup defaults
	SurfaceModel model;
	model.Albedo = float3(0, 0, 0);
	model.Normal = float3(0, 0, 1);
	model.Metallic = 0;
	model.Roughness = 0.5;
	model.Specular = 0.5;
	model.Emissive = float3(0, 0, 0);
	model.Opacity = 1;
	model.ShadingModel = SHADING_LIT;
	
	// Read material params from buffer.
	uint shaderID = MaterialParams.Load(materialID + 0);
	
	// Create inputs structure
	SFInput input;
	input.UV0 = uv0;
	input.DDX = ddx;
	input.DDY = ddy;
	
	#pragma SETUP
	
	// Invoke surface shader
	SFMain(model, input);
	return model;
}