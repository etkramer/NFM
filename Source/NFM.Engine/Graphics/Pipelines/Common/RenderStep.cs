using System;
using NFM.GPU;
using NFM.World;

namespace NFM.Graphics;

abstract class RenderStep
{
	public virtual void Init() {}
	public abstract void Run(CommandList list);
}

abstract class SceneStep : RenderStep
{
	public Scene Scene { get; set; }
}

abstract class CameraStep<T> : RenderStep where T : RenderPipeline<T>, new()
{
	public T RP { get; set; }
	public CameraNode Camera { get; set; }
}