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
	uint MeshletID : MESHLETID;
	uint TriangleID : TRIANGLEID;
};

uint3 GetPrimitive(uint offset)
{
	return uint3(Primitives[offset], Primitives[offset + 1], Primitives[offset + 2]);
}

Vertex GetVertex(Mesh mesh, Meshlet meshlet, uint vert)
{
	return Vertices[mesh.VertStart + meshlet.VertStart + vert];
}