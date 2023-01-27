#include "Shaders/World.h"

struct IndirectCommand
{
	uint InstanceID;
	uint ThreadGroupCountX;
	uint ThreadGroupCountY;
	uint ThreadGroupCountZ;
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
	bool visible = mesh.MeshletCount > 0;
	if (visible)
	{
		// Build command.
		IndirectCommand command;
		command.InstanceID = instanceID;
		command.ThreadGroupCountX = mesh.MeshletCount;
		command.ThreadGroupCountY = 1;
		command.ThreadGroupCountZ = 1;

		// Store command and update count.
		Commands.Append(command);
	}
}