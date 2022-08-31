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
		Scene,
		Viewport,
	}

	public abstract class RenderStep
	{
		public static Viewport Viewport;
		public static Scene Scene;
		public static CommandList List;

		public virtual void Init() {}
		public abstract void Run();
	}
}
