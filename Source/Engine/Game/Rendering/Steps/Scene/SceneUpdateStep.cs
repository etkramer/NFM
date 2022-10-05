using System;
using Engine.World;

namespace Engine.Rendering
{
	public class SceneUpdateStep : RenderStep
	{
		public override void Run()
		{
			// Update actor instances.
			foreach (Node actor in Scene.Nodes)
			{
				RecurseInstances(actor);
			}

			// Make sure the instance buffer is fully compacted.
			List.CompactBuffer(Scene.InstanceBuffer);
		}

		// Loops through actors and (re)uploads instance data where requested.
		private void RecurseInstances(Node root)
		{
			if (root.IsTransformDirty)
			{
				root.UpdateTransform();
			}

			if (root is ModelNode modelActor)
			{
				if (modelActor.IsInstanceDirty)
				{
					modelActor.UpdateInstances();
				}
			}

			if (root.Children.Count > 0)
			{
				foreach (Node child in root.Children)
				{
					RecurseInstances(child);
				}
			}
		}
	}
}