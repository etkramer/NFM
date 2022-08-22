#include "Shaders/Include/Geometry.hlsl"
#include "Shaders/Include/Common.hlsl"

Texture2D<uint2> VisBuffer : register(t0);
Texture2D<float> DepthBuffer : register(t1);
RWTexture2D<float4> RenderTarget : register(u0);

float3 IndexToColor(uint i)
{
	if (i % 6 == 0)
	{
		return float3(0.82, 0.8, 0.57);
	}
	else if (i % 6 == 1)
	{
		return float3(0.58, 0.37, 0.87);
	}
	else if (i % 6 == 2)
	{
		return float3(0.88, 0.07, 0.6);
	}
	else if (i % 6 == 3)
	{
		return float3(0.89, 0.89, 0.14);
	}
	else if (i % 6 == 4)
	{
		return float3(0.58, 0.86, 0.89);
	}
	else
	{
		return float3(0, 0.47, 0.84);
	}
}

[numthreads(32, 32, 1)]
void ComputeEntry(uint3 ID : SV_DispatchThreadID)
{
	// Grab the frame width/height.
	int2 frameSize;
	RenderTarget.GetDimensions(frameSize.x, frameSize.y);

	// Make sure we don't try to read outside the bounds of the RT.
	if (ID.x >= frameSize.x || ID.y >= frameSize.y)
	{
		return;
	}

	// Early out if this pixel is empty.
	bool isEmpty = DepthBuffer[ID.xy] == 0;
	if (isEmpty)
	{
		return;
	}

	// Unpack data from visbuffer.
	uint2 unpacked = BitUnpack(VisBuffer[ID.xy][1], 25);
	uint instanceID = VisBuffer[ID.xy][0];
	uint meshletID = unpacked[0];
	uint triangleID = unpacked[1];

	Instance inst = Instances[instanceID];
	Mesh mesh = Meshes[inst.Mesh];
	Meshlet meshlet = Meshlets[mesh.MeshletStart + meshletID];

	// Write it to the render target.
	RenderTarget[ID.xy] = float4(IndexToColor(instanceID), 1);
}