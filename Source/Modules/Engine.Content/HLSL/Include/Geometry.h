// Shared view constants
struct _ViewConstants
{
	float4x4 WorldToView;
	float4x4 ViewToWorld;

	float4x4 ViewToClip;
	float4x4 ClipToView;
};
ConstantBuffer<_ViewConstants> ViewConstants : register(b256);

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
};

// Global geometry data - keep slot numbers out of the way. Should probably use spaces instead.
StructuredBuffer<Transform> Transforms : register(t251);
StructuredBuffer<Instance> Instances : register(t252);
StructuredBuffer<Mesh> Meshes : register(t253);
StructuredBuffer<Meshlet> Meshlets : register(t254);
StructuredBuffer<uint> Primitives : register(t255);
StructuredBuffer<Vertex> Vertices : register(t256);