using System;
using Engine.Content;
using Engine.GPU;
using Engine.Resources;
using Engine.World;

namespace Engine.Rendering
{
	public class SceneUpdateStep : RenderStep
	{
		public override void Run()
		{
			// Update actor instances.
			foreach (Actor actor in Scene.Main.Actors)
			{
				RecurseInstances(actor);
			}
		}

		// Loops through actors and (re)uploads instance data where requested.
		private void RecurseInstances(Actor root)
		{
			if (root is ModelActor modelActor)
			{
				if (modelActor.IsInstanceDirty)
				{
					modelActor.UpdateInstances();
				}
			}

			if (root.Children.Count > 0)
			{
				foreach (Actor child in root.Children)
				{
					RecurseInstances(child);
				}
			}
		}
	}
}