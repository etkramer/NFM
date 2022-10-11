using System;
using Engine.GPU;
using Engine.World;

namespace Engine.Rendering
{
	public abstract class RenderStep
	{
		public virtual void Init() {}
		public abstract void Run();
	}

	public abstract class SceneStep : RenderStep
	{
		public Scene Scene { get; set; }
		public CommandList List => Renderer.DefaultCommandList;
	}

	public abstract class CameraStep : RenderStep
	{
		public RenderTarget RT { get; set; }
		public CameraNode Camera { get; set; }
		public CommandList List => RT.CommandList;
	}
}
