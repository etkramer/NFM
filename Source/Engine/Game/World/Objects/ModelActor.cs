using System;
using Engine.GPU;
using Engine.Rendering;
using Engine.Resources;

namespace Engine.World
{
	public class ModelActor : Actor
	{
		// GPU-side instance buffer.
		internal static GraphicsBuffer<Instance> InstanceBuffer = new(2000000);
		internal static int InstanceCount = 0;

		[StructLayout(LayoutKind.Sequential)]
		internal struct Instance
		{
			public uint Mesh;
			public Matrix4 Transform;
		}

		[Inspect] public Model Model { get; set; } = null;
		
		internal List<BufferHandle<Instance>> Instances = new();
		internal bool IsInstanceDirty = true;
		
		public ModelActor(string name = null) : base(name)
		{
			(this as INotify).Subscribe(nameof(Model), () => IsInstanceDirty = true);
			(this as INotify).Subscribe(nameof(Position), () => IsInstanceDirty = true);
			(this as INotify).Subscribe(nameof(Rotation), () => IsInstanceDirty = true);
			(this as INotify).Subscribe(nameof(Scale), () => IsInstanceDirty = true);
		}

		public override void Dispose()
		{
			FreeInstances();
			base.Dispose();
		}

		internal void UpdateInstances()
		{
			if (Model == null ||Model?.Parts == null || !IsInstanceDirty)
			{
				return;
			}

			for (int i = Instances.Count - 1; i >= 0; i--)
			{
				Instances[i].Free();
				Instances.RemoveAt(i);
				InstanceCount--;
			}

			// A Model can contain multiple ModelParts, which in turn may contain multiple submeshes. Every submesh needs it's own instance.
			foreach (ModelPart part in Model.Parts)
			{
				foreach (Submesh submesh in part.Submeshes)
				{
					// Make instance data.
					Instance instanceData = new()
					{
						Mesh = (uint)submesh.MeshHandle.ElementStart,
						Transform = Matrix4.CreateTransform(Position, Rotation, Scale)
					};

					// Upload instance.
					Instances.Add(InstanceBuffer.Upload(instanceData));
					InstanceCount++;
				}
			}

			IsInstanceDirty = false;
		}

		private void FreeInstances()
		{
			foreach (var instance in Instances)
			{
				instance.Free();
				InstanceCount--;
			}
		}
	}
}