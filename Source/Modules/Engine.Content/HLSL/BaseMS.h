#include "Include/Geometry.h"

struct VertAttribute
{
	float4 Position : SV_POSITION;
	float4 Normal : NORMAL;
};

struct PrimAttribute
{
	uint PrimitiveID : PRIMITIVEID;
	uint InstanceID : INSTANCEID;
	uint MeshletID : MESHLETID;
};

uint3 GetPrimitive(Mesh mesh, Meshlet meshlet, uint prim)
{
	uint prim0 = mesh.PrimStart + meshlet.PrimStart + (prim * 3);
	uint prim1 = prim0 + 1;
	uint prim2 = prim0 + 2;
	return uint3(Primitives[prim0], Primitives[prim1], Primitives[prim2]);
}

Vertex GetVertex(Mesh mesh, Meshlet meshlet, uint vert)
{
	return Vertices[mesh.VertStart + meshlet.VertStart + vert];
}