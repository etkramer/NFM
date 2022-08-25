#include "Shaders/Include/Geometry.h"

struct IndirectCommand
{
	uint InstanceID;
	uint ThreadGroupCountX;
	uint ThreadGroupCountY;
	uint ThreadGroupCountZ;
};

RWStructuredBuffer<IndirectCommand> Commands : register(u0);
RWStructuredBuffer<uint> CommandCount : register(u1);

[numthreads(1, 1, 1)]
void ComputeEntry(uint3 dispatchID : SV_DispatchThreadID)
{
	// Grab instance/mesh data.
	Instance instance = Instances[dispatchID.x];
	Mesh mesh = Meshes[instance.Mesh];
	
	bool visible = mesh.MeshletCount > 0;
	if (visible)
	{
		// Build command.
		IndirectCommand command;
		command.InstanceID = dispatchID.x;
		command.ThreadGroupCountX = mesh.MeshletCount;
		command.ThreadGroupCountY = 1;
		command.ThreadGroupCountZ = 1;

		// Store command and update count.
		uint commandID = dispatchID.x;
		Commands[commandID] = command;
		InterlockedMax(CommandCount[0], commandID + 1);
	}
}