#include "Shaders/Standard/Culling.h"

ByteAddressBuffer MaterialParams : register(t0);
AppendStructuredBuffer<IndirectCommand> Commands : register(u0);

// Which blend modes this pass draws, as InstanceBucketBit flags.
uint BucketMask : register(b0);

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

	// Drawn by whichever pass claims this blend mode.
	if ((InstanceBucketBit(instance) & BucketMask) == 0)
	{
		return;
	}

	Mesh mesh = Meshes[instance.MeshID];

	// Check visibility.
	if (IsInstanceVisible(instance, mesh))
	{
		// Store command and update count.
		Commands.Append(BuildDrawCommand(instanceID, mesh));
	}
}
