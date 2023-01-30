#include "Shaders/World.h"

struct IndirectCommand
{
	uint InstanceID;
	
	uint IndexCountPerInstance;
	uint InstanceCount;
	uint StartIndexLocation;
	uint BaseVertexLocation;
	uint StartInstanceLocation;
};

ByteAddressBuffer MaterialParams : register(t0);
AppendStructuredBuffer<IndirectCommand> Commands : register(u0);

[numthreads(1, 1, 1)]
void main(uint3 dispatchID : SV_DispatchThreadID)
{
	// Grab instance/mesh data.
	uint instanceID = dispatchID.x;
	Instance instance = Instances[instanceID];

	// Invalid mesh reference, instance has probably been zeroed.
	if (instance.MeshID == 0)
	{
		return;
	}

	Mesh mesh = Meshes[instance.MeshID];

	// Check visibility.
	bool visible = true;
	if (visible)
	{
		// Build command.
		IndirectCommand command;
		command.InstanceID = instanceID;
		
		command.IndexCountPerInstance = mesh.IndexCount;
		command.InstanceCount = 1;
		command.StartIndexLocation = mesh.IndexOffset;
		command.BaseVertexLocation = mesh.VertexOffset;
		command.StartInstanceLocation = 0;

		// Store command and update count.
		Commands.Append(command);
	}
}