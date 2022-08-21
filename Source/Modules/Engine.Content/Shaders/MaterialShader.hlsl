#include "Shaders/Include/Geometry.hlsl"

RWTexture2D<float4> RenderTarget : register(u0);

[numthreads(32, 32, 1)]
void ComputeEntry(uint3 ID : SV_DispatchThreadID)
{
	uint2 pixel = uint2(ID.x, ID.y);
	//SurfaceInfo surface = Surface();

	float4 output = float4(1, 0, 1, 1);
	RenderTarget[pixel] = output;
}