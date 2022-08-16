#include "Shaders/Include/Geometry.hlsl"
#include "Shaders/Include/Outputs.hlsl"

float4 PixelEntry(MeshOut input) : SV_TARGET
{
	Mesh mesh = Meshes[Instances[input.InstanceID].Mesh];
	Meshlet meshlet = Meshlets[mesh.MeshletStart + input.MeshletID];

	float4 meshletColor;
	if (input.MeshletID % 6 == 0)
	{
		meshletColor = float4(0.82, 0.8, 0.57, 1);
	}
	else if (input.MeshletID % 6 == 1)
	{
		meshletColor = float4(0.58, 0.37, 0.87, 1);
	}
	else if (input.MeshletID % 6 == 2)
	{
		meshletColor = float4(0.88, 0.07, 0.6, 1);
	}
	else if (input.MeshletID % 6 == 3)
	{
		meshletColor = float4(0.89, 0.89, 0.14, 1);
	}
	else if (input.MeshletID % 6 == 4)
	{		
		meshletColor = float4(0.58, 0.86, 0.89, 1);
	}
	else
	{
		meshletColor = float4(0, 0.47, 0.84, 1);
	}

	return meshletColor;
}

/*static const int NUM_TRIANGLE_BITS = 17;

uint PixelEntry(MeshOut input) : SV_TARGET
{
	uint objectID = input.InstanceID;
	uint triangleID = Meshlets[input.MeshletID].PrimStart + input.TriangleID;
	uint packedResult = (objectID << NUM_TRIANGLE_BITS | triangleID);

	return packedResult;
}*/