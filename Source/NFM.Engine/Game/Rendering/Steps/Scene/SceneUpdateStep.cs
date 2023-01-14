using System;
using NFM.World;

namespace NFM.Rendering
{
	public class SceneUpdateStep : SceneStep
	{
		public override void Run()
		{
			// Loop through nodes and (re)upload instance data where requested.
			foreach (Node node in Scene.EnumerateNodes())
			{
				if (node is ModelNode model)
				{
					if (!model.IsInstanceValid)
					{
						model.UpdateInstances(List);
					}
				}
			}
		}
	}
}