using System;
using Engine.World;

namespace Engine.Rendering
{
	public class SceneUpdateStep : RenderStep
	{
		public override void Run()
		{
			// Loop through nodes and (re)upload instance data where requested.
			foreach (Node node in Scene.Nodes)
			{
				if (node.IsTransformDirty)
				{
					node.UpdateTransform();
				}

				if (node is ModelNode model)
				{
					if (model.IsInstanceDirty)
					{
						model.UpdateInstances();
					}
				}
			}

			// Make sure the instance buffer is fully compacted.
			List.CompactBuffer(Scene.InstanceBuffer);
		}
	}
}