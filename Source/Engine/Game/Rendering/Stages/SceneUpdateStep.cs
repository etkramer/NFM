using System;
using Engine.Content;
using Engine.GPU;
using Engine.Resources;
using Engine.World;

namespace Engine.Rendering
{
	[StructLayout(LayoutKind.Sequential)]
	internal struct Instance
	{
		public uint Mesh;
		public Matrix4 Transform;
	}

	public class SceneUpdateStep : RenderStep
	{
		internal static GraphicsBuffer<Instance> InstanceBuffer = new(2000000);
		internal static int InstanceCount = 0;

		public override void Run()
		{
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
					UpdateInstance(modelActor);
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

		// Upload instance data to the GPU.
		private void UpdateInstance(ModelActor modelActor)
		{
			if (modelActor.Model == null || !modelActor.IsInstanceDirty)
			{
				return;
			}

			for (int i = modelActor.Instances.Count - 1; i >= 0; i--)
			{
				modelActor.Instances[i].Free();
				modelActor.Instances.RemoveAt(i);
				InstanceCount--;
			}

			// A Model can contain multiple ModelParts, which in turn may contain multiple submeshes. Every submesh needs it's own instance.
			foreach (ModelPart part in modelActor.Model.Parts)
			{
				foreach (Submesh submesh in part.Submeshes)
				{
					// Make instance data.
					Instance instanceData = new()
					{
						Mesh = (uint)submesh.MeshHandle.ElementStart,
						Transform = Matrix4.CreateTransform(modelActor.Position, modelActor.Rotation, modelActor.Scale)
					};

					// Upload instance.
					modelActor.Instances.Add(InstanceBuffer.Upload(instanceData));
					InstanceCount++;
				}
			}

			modelActor.IsInstanceDirty = false;
		}
	}
}