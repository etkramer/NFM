#include "Shaders/Common/Gizmos/Gizmos.h"

float4 main(VertexAttribute vert) : SV_TARGET0
{
	return vert.Color;
}