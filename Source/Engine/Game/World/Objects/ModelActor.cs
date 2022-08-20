using System;
using Engine.GPU;
using Engine.Rendering;
using Engine.Resources;

namespace Engine.World
{
	public class ModelActor : Actor
	{
		[Inspect] public Model Model { get; set; } = null;

		internal List<BufferHandle<Instance>> Instances = new();
		internal bool IsInstanceDirty = true;
		
		public ModelActor(string name = null, Actor parent = null) : base(name, parent)
		{
			(this as INotify).Subscribe(nameof(Model), () => IsInstanceDirty = true);
			(this as INotify).Subscribe(nameof(Position), () => IsInstanceDirty = true);
			(this as INotify).Subscribe(nameof(Rotation), () => IsInstanceDirty = true);
			(this as INotify).Subscribe(nameof(Scale), () => IsInstanceDirty = true);
		}

		public override void Dispose()
		{
			foreach (var instance in Instances)
			{
				instance.Free();
			}

			base.Dispose();
		}
	}
}