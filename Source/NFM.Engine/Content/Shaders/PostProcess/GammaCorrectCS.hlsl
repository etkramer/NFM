#include "Content/Shaders/Common.h"

RWTexture2D<float4> Image : register(u0);

[numthreads(32, 32, 1)]
void GammaCorrectCS(uint3 pixel : SV_DispatchThreadID)
{
	int width, height;
	Image.GetDimensions(width, height);

	// Don't try to process out of bounds pixels.
	if (pixel.x >= width || pixel.y >= height)
	{
		return;
	}

	// Apply gamma correction.
	Image[pixel.xy] = SRGBToLinear(Image[pixel.xy]);
}