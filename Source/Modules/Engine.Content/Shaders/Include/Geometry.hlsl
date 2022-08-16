// One per object in scene (unordered, compact).
struct Instance
{
	uint Mesh;
	float4x4 Transform;
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

// Geometry data
StructuredBuffer<Instance> Instances : register(t0);

StructuredBuffer<Mesh> Meshes : register(t1);
StructuredBuffer<Meshlet> Meshlets : register(t2);
StructuredBuffer<uint> Primitives : register(t3);
StructuredBuffer<Vertex> Vertices : register(t4);