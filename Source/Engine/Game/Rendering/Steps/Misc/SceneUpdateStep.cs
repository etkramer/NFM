using System;
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

			// Make sure the instance buffer is fully compacted.
			List.CompactBuffer(Scene.InstanceBuffer);

			// Update view data.
			foreach (Viewport viewport in Viewport.All)
			{
				viewport.UpdateView();
			}
		}

		// Loops through actors and (re)uploads instance data where requested.
		private void RecurseInstances(Actor root)
		{
			if (root.IsTransformDirty)
			{
				root.UpdateTransform();
			}

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