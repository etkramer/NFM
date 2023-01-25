// Shared view constants
struct _ViewConstants
{
	float4x4 WorldToView;
	float4x4 ViewToWorld;

	float4x4 ViewToClip;
	float4x4 ClipToView;

	float2 ViewportSize;
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
	uint VertStart; // Start of submesh in vertex buffer.
	uint PrimStart; // Start of submesh in primitive buffer.
	uint MeshletStart; // Start of submesh in meshlet buffer.
	uint MeshletCount;   // Number of meshlets used
};

// (Mesh.MeshletCount) per loaded mesh.
struct Meshlet
{
	uint VertStart; // First vertex in vertex buffer (relative to start of submesh)
	uint PrimStart;   // First primitive in primitive buffer (relative to start of submesh)
	uint VertCount; // Number of vertices used
	uint PrimCount;   // Number of primitives used
};

struct Vertex
{
	float3 Position;
	float3 Normal;
	float2 UV0;
	float2 UV1;
};

// Global geometry data.
StructuredBuffer<Vertex> Vertices : register(t0, space1);
StructuredBuffer<uint> Primitives : register(t1, space1);
StructuredBuffer<Meshlet> Meshlets : register(t2, space1);
StructuredBuffer<Mesh> Meshes : register(t3, space1);
StructuredBuffer<Transform> Transforms : register(t4, space1);
StructuredBuffer<Instance> Instances : register(t5, space1);