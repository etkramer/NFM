Texture2D<uint2> VisBuffer : register(t0);
Texture2D<float> DepthBuffer : register(t1);

RWStructuredBuffer<uint2> PickResult : register(u0);

int2 PickCoords : register(b0);

[numthreads(1, 1, 1)]
void main()
{
	// VisBuffer is never cleared, so depth is what tells us whether anything was rasterized here.
	if (DepthBuffer[PickCoords] == 0)
	{
		PickResult[0] = uint2(0, 0);
		return;
	}

	PickResult[0] = uint2(1, VisBuffer[PickCoords].x);
}
