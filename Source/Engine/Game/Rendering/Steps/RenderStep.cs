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
		public Viewport Viewport { get; set; }
		public Scene Scene { get; set; }
		public CommandList List { get; set; }

		public virtual void Init() {}
		public abstract void Run();
	}
}
