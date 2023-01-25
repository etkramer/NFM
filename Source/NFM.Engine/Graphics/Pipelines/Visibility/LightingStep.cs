using System;
using NFM.GPU;

namespace NFM.Graphics;

public class LightingStep : CameraStep<VisibilityRenderPipeline>
{
	public override void Init()
	{
		// #include SURFACE
		// Calculate PBR/lighting
		// Output normals for convenience
	}

	public override void Run(CommandList list)
	{

	}
}