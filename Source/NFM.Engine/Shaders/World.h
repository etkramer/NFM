#pragma once

// Shared view constants
struct _ViewConstants
{
	float4x4 WorldToView;
	float4x4 ViewToWorld;

	float4x4 ViewToClip;
	float4x4 ClipToView;

	float3 EyePosition;

	float ClusterScale;
	uint3 ClusterDims;
	float ClusterBias;

	float InvLightCutoff; // Reciprocal of the illuminance a light stops being worth evaluating at
};
ConstantBuffer<_ViewConstants> ViewConstants : register(b0, space1);

#define INSTANCE_BLEND_MASK 0x3
#define INSTANCE_BLEND_OPAQUE 0
#define INSTANCE_BLEND_MASKED 1
#define INSTANCE_BLEND_OVER 2
#define INSTANCE_BLEND_ADDITIVE 3

// One per object in scene (unordered, compact).
struct Instance
{
	uint MeshID;
	uint MaterialID;
	uint TransformID;
	uint VertexOffset; // Start of this instance's vertices, deformed or shared with the mesh
	uint Flags; // INSTANCE_ bits, blend mode in the low two
	uint Pad;
	uint2 BLASAddress; // Structure to trace against, deformed or shared with the mesh
};

// The instance's blend mode as a single bit, for testing against a bucket mask.
uint InstanceBucketBit(Instance instance)
{
	return 1u << (instance.Flags & INSTANCE_BLEND_MASK);
}

struct Transform
{
	float4x4 ObjectToWorld;
	float4x4 WorldToObject;
};

// One per loaded mesh.
struct Mesh
{
	uint VertexOffset; // Start of vertices in vertex buffer
	uint IndexOffset; // Start of indices in index buffer
	uint IndexCount; // Number of indices in index buffer
};

// One per light in scene (unordered, sparse).
struct Light
{
	uint Type; // LIGHT_ constant, or LIGHT_NONE for an empty slot
	float3 Position;
	float3 Color; // Linear RGB, scaled by intensity in candela
	float Radius; // Source radius, in meters
};

struct Vertex
{
	float3 Position;
	float3 Normal;
	float4 Tangent;
	float2 UV0;
	float2 UV1;
};

// Bone influences for one vertex, four indices and four weights packed as ushorts.
struct VertexWeights
{
	uint2 Indices;
	uint2 Weights;
};

uint UnpackWeightIndex(VertexWeights weights, uint influence)
{
	return (weights.Indices[influence >> 1] >> ((influence & 1) * 16)) & 0xFFFF;
}

float UnpackWeight(VertexWeights weights, uint influence)
{
	return ((weights.Weights[influence >> 1] >> ((influence & 1) * 16)) & 0xFFFF) / 65535.0;
}

// Global geometry data.
StructuredBuffer<Vertex> Vertices : register(t0, space1);
StructuredBuffer<uint> Indices : register(t1, space1);
StructuredBuffer<VertexWeights> Weights : register(t2, space1);
StructuredBuffer<Mesh> Meshes : register(t3, space1);
StructuredBuffer<Transform> Transforms : register(t4, space1);
StructuredBuffer<Instance> Instances : register(t5, space1);
StructuredBuffer<Light> Lights : register(t6, space1);
StructuredBuffer<float4x4> Bones : register(t7, space1);

// Recovers the world position a pixel was shaded at, from reversed-Z depth.
float3 ReconstructWorldPosition(uint2 id, int2 frameSize, float depth)
{
	float2 ndc = ((float2(id) + 0.5) / float2(frameSize)) * 2 - 1;
	ndc.y *= -1;

	float4 viewPos = mul(ViewConstants.ClipToView, float4(ndc, depth, 1));
	return mul(ViewConstants.ViewToWorld, float4(viewPos.xyz / viewPos.w, 1)).xyz;
}