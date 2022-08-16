#include "Shaders/Include/Geometry.hlsl"

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
void ComputeEntry(uint groupID : SV_GroupID)
{
	// Grab instance/mesh data.
	Instance instance = Instances[groupID];
	Mesh mesh = Meshes[instance.Mesh];
	
	bool visible = mesh.MeshletCount > 0;
	if (visible)
	{
		// Build command.
		IndirectCommand command;
		command.InstanceID = groupID;
		command.ThreadGroupCountX = mesh.MeshletCount;
		command.ThreadGroupCountY = 1;
		command.ThreadGroupCountZ = 1;

		// Store command and update count.
		uint commandID = groupID;
		Commands[commandID] = command;
		InterlockedAdd(CommandCount[0], 1);
	}
}