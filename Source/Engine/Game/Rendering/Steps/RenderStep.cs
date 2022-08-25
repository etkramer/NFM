using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.GPU;
using Engine.World;

namespace Engine.Rendering
{
	public enum RenderStage
	{
		Global,
		Viewport,
	}

	public abstract class RenderStep
	{
		public static Viewport Viewport;
		public static Scene Scene;

		public static CommandList List => Viewport?.CommandList ?? Graphics.GetCommandList();

		public virtual void Init() {}
		public abstract void Run();
	}
}
