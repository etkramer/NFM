#pragma once

#define SCAN_GROUP_SIZE 256

groupshared uint Partials[SCAN_GROUP_SIZE];

// Turns Partials into an inclusive scan of itself.
void ScanPartials(uint threadID)
{
	for (uint offset = 1; offset < SCAN_GROUP_SIZE; offset <<= 1)
	{
		uint sum = Partials[threadID];
		if (threadID >= offset)
		{
			sum += Partials[threadID - offset];
		}

		GroupMemoryBarrierWithGroupSync();
		Partials[threadID] = sum;
		GroupMemoryBarrierWithGroupSync();
	}
}
