using System;
using Engine.Content;
using Engine.GPU;
using Engine.Resources;
using Engine.World;

namespace Engine.Rendering
{
	public class ResolveStep : RenderStep
	{
		public override void Init()
		{

		}

		public override void Run()
		{
			// Copy output to backbuffer.
			Graphics.CopyTexture(Viewport.ColorTarget, Viewport.Host.Swapchain.RT);

			// Clear viewport targets.
			Graphics.ClearRenderTarget(Viewport.ColorTarget);
			Graphics.ClearDepth(Viewport.DepthBuffer);
		}
	}
}