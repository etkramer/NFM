using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

		public virtual void Init() {}
		public abstract void Run();
	}
}
