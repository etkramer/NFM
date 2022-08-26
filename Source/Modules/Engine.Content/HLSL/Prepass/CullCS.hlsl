#include "HLSL/Include/Geometry.h"

struct IndirectCommand
{
	uint InstanceID;
	uint ThreadGroupCountX;
	uint ThreadGroupCountY;
	uint ThreadGroupCountZ;
};

AppendStructuredBuffer<IndirectCommand> Commands : register(u0);

[numthreads(1, 1, 1)]
void CullCS(uint3 dispatchID : SV_DispatchThreadID)
{
	// Grab instance/mesh data.
	uint instanceID = dispatchID.x;
	Instance instance = Instances[instanceID];
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