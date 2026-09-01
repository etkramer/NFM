#include "Shaders/World.h"

RWStructuredBuffer<Vertex> RWVertices : register(u0);

cbuffer SkinParams : register(b0)
{
	uint SourceOffset; // Start of the mesh's bind-pose vertices
	uint DestOffset; // Start of this instance's deformed vertices
	uint WeightOffset;
	uint BoneOffset;
	uint VertexCount;
};

[numthreads(64, 1, 1)]
void main(uint3 dispatchID : SV_DispatchThreadID)
{
	uint vertexID = dispatchID.x;
	if (vertexID >= VertexCount)
	{
		return;
	}

	Vertex vertex = RWVertices[SourceOffset + vertexID];
	VertexWeights weights = Weights[WeightOffset + vertexID];

	// Weights sum to one, so the influences blend into a single skinning matrix.
	float4x4 skin = 0;
	float total = 0;

	[unroll]
	for (uint i = 0; i < 4; i++)
	{
		float weight = UnpackWeight(weights, i);
		skin += Bones[BoneOffset + UnpackWeightIndex(weights, i)] * weight;
		total += weight;
	}

	// An uninfluenced vertex rides along with the model rather than collapsing to the origin.
	if (total < 1e-4)
	{
		skin = float4x4(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);
	}

	float3x3 skin3 = (float3x3)skin;

	vertex.Position = mul(skin, float4(vertex.Position, 1)).xyz;
	vertex.Normal = normalize(mul(skin3, vertex.Normal));
	vertex.Tangent = float4(normalize(mul(skin3, vertex.Tangent.xyz)), vertex.Tangent.w);

	RWVertices[DestOffset + vertexID] = vertex;
}
