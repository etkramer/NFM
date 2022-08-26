uint BitPack(uint low, uint high, int lowBits)
{
	return (high << lowBits) | low;
}

uint2 BitUnpack(uint packed, int lowBits)
{
	uint low = packed & ((1U << lowBits) - 1U);
	uint high = packed >> lowBits;
	return uint2(low, high);
}

float3 IndexToColor(uint i)
{
	if (i % 6 == 0)
	{
		return float3(0.82, 0.8, 0.57);
	}
	else if (i % 6 == 1)
	{
		return float3(0.58, 0.37, 0.87);
	}
	else if (i % 6 == 2)
	{
		return float3(0.88, 0.07, 0.6);
	}
	else if (i % 6 == 3)
	{
		return float3(0.89, 0.89, 0.14);
	}
	else if (i % 6 == 4)
	{
		return float3(0.58, 0.86, 0.89);
	}
	else
	{
		return float3(0, 0.47, 0.84);
	}
}