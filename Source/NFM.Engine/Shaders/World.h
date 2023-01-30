// Shared view constants
struct _ViewConstants
{
	float4x4 WorldToView;
	float4x4 ViewToWorld;

	float4x4 ViewToClip;
	float4x4 ClipToView;

	float3 EyePosition;
};
ConstantBuffer<_ViewConstants> ViewConstants : register(b0, space1);

// One per object in scene (unordered, compact).
struct Instance
{
	uint MeshID;
	uint MaterialID;
	uint TransformID;
};

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

struct Vertex
{
	float3 Position;
	float3 Normal;
	float4 Tangent;
	float2 UV0;
	float2 UV1;
};

// Global geometry data.
StructuredBuffer<Vertex> Vertices : register(t0, space1);
StructuredBuffer<uint> Indices : register(t1, space1);
StructuredBuffer<Mesh> Meshes : register(t3, space1);
StructuredBuffer<Transform> Transforms : register(t4, space1);
StructuredBuffer<Instance> Instances : register(t5, space1);