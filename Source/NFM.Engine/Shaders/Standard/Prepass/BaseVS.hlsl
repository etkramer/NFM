#include "Shaders/World.h"

int InstanceID : register(b0);

float4 main(uint vertexID : SV_VertexID) : SV_Position
{
	// Grab instance data.
	Instance instance = Instances[InstanceID];
	Mesh mesh = Meshes[instance.MeshID];
	
	Vertex vertex = Vertices[mesh.VertexOffset + vertexID];
	
	// Grab the transform matrix.
	Transform transform = Transforms[instance.TransformID];
	
	// Apply projection to vertex position.
	float4 position = float4(vertex.Position, 1);
	position = mul(transform.ObjectToWorld, position); // Apply instance transform.
	position = mul(ViewConstants.WorldToView, position); // Apply camera view.
	position = mul(ViewConstants.ViewToClip, position); // Apply camera projection.
	
	return position;
}