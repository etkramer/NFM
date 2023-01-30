#include "Shaders/Common.h"
#include "Shaders/World.h"

int InstanceID : register(b0);

uint2 main(uint primitiveID : SV_PrimitiveID) : SV_TARGET0
{
	return uint2(InstanceID, primitiveID);
}