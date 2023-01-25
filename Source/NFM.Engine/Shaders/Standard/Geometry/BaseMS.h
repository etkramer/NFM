#include "../../Geometry.h"

struct VertAttribute
{
	float4 Position : SV_POSITION;
	float3 Normal : NORMAL;
	float2 UV0 : UV0;
};

struct PrimAttribute
{
	uint InstanceID : INSTANCEID;
	uint TriangleID : TRIANGLEID;
};

uint3 GetPrimitive(uint offset)
{
	uint prim0 = offset;
	uint prim1 = offset + 1;
	uint prim2 = offset + 2;
	return uint3(Primitives[prim0], Primitives[prim1], Primitives[prim2]);
}

Vertex GetVertex(Mesh mesh, Meshlet meshlet, uint vert)
{
	return Vertices[mesh.VertStart + meshlet.VertStart + vert];
}