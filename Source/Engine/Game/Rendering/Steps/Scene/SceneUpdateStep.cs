using System;
using Engine.World;

namespace Engine.Rendering
{
	public class SceneUpdateStep : SceneStep
	{
		public override void Run()
		{
			// Loop through nodes and (re)upload instance data where requested.
			foreach (Node node in Scene.Nodes)
			{
				if (node is ModelNode model)
				{
					if (model.IsTransformDirty)
					{
						model.UpdateTransform(List);
					}

					if (model.IsInstanceDirty)
					{
						model.UpdateInstances(List);
					}
				}
			}
		}
	}
}