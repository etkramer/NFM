#include "Content/Shaders/Gizmos/Gizmos.h"

float4 GizmosPS(VertexAttribute vert) : SV_TARGET0
{
	return vert.Color;
}