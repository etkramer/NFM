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