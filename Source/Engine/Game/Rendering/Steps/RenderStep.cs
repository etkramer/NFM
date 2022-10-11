using System;
using Engine.GPU;
using Engine.World;

namespace Engine.Rendering
{
	public enum RenderStage
	{
		Global,
		Scene,
		Camera,
	}

	public abstract class RenderStep
	{
		public RenderTarget RT { get; set; }
		public CameraNode Camera { get; set; }
		public Scene Scene { get; set; }
		public CommandList List { get; set; }

		public virtual void Init() {}
		public abstract void Run();
	}
}
