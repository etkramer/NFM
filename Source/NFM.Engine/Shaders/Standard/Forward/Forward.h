#pragma once

#include "Shaders/World.h"

int InstanceID : register(b0);

struct ForwardVertex
{
	float4 Position : SV_Position;
	float3 WorldPos : WORLDPOS;
	float3 Normal : NORMAL;
	float4 Tangent : TANGENT; // Handedness in w, matching the vertex data
	float2 UV0 : TEXCOORD0;
};
