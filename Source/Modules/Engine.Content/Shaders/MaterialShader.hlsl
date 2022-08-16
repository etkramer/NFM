#include "Shaders/Include/Geometry.hlsl"

[numthreads(128, 128, 1)]
void ComputeEntry(uint3 ID : SV_DispatchThreadID)
{
	SurfaceInfo surface = Surface();
}