#pragma once

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

IndirectCommand BuildDrawCommand(uint instanceID, Mesh mesh)
{
	IndirectCommand command;
	command.InstanceID = instanceID;

	command.IndexCountPerInstance = mesh.IndexCount;
	command.InstanceCount = 1;
	command.StartIndexLocation = mesh.IndexOffset;
	command.BaseVertexLocation = 0;
	command.StartInstanceLocation = 0;

	return command;
}

bool IsInstanceVisible(Instance instance, Mesh mesh)
{
	return true;
}
