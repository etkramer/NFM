using System;
using Engine.GPU;
using Engine.Rendering;
using Engine.Resources;

namespace Engine.World
{
	[StructLayout(LayoutKind.Sequential)]
	public struct InstanceData
	{
		public uint MeshID;
		public uint MaterialID;
		public uint TransformID;
	}

	public partial class ModelActor : Actor
	{
		public const int MaxInstanceCount = 100;
		public static int InstanceCount = 0;	
		public static GraphicsBuffer<InstanceData> InstanceBuffer = new(MaxInstanceCount);

		[Inspect] public Model Model { get; set; } = null;
	
		// Mesh instances
		public bool IsInstanceDirty = true;
		public BufferHandle<InstanceData>[] InstanceHandles;

		// Material instances
		public MaterialInstance[] MaterialInstances;
		
		public ModelActor(string name = null) : base(name)
		{
			(this as INotify).Subscribe(nameof(Model), () => IsInstanceDirty = true);
		}

		public override void Dispose()
		{
			for (int i = 0; i < InstanceHandles?.Length; i++)
			{
				MaterialInstances[i].Dispose();
				InstanceHandles[i].Dispose();
				InstanceCount--;
			}

			base.Dispose();
		}

		public void UpdateInstances()
		{
			if (Model == null ||Model?.Parts == null)
			{
				InstanceHandles = null;
				MaterialInstances = null;
				return;
			}

			// Count instances.
			uint instanceCount = 0;
			foreach (ModelPart part in Model.Parts)
			{
				foreach (Mesh mesh in part.Meshes)
				{
					instanceCount++;
				}
			}

			// (Re)build the array of instance handles.
			if (InstanceHandles == null || InstanceHandles.Length != instanceCount)
			{
				MaterialInstances?.ForEach(o => o.Dispose());
				InstanceHandles?.ForEach(o => o.Dispose());

				// Allocate a handful of new ones.
				MaterialInstances = new MaterialInstance[instanceCount];
				InstanceHandles = new BufferHandle<InstanceData>[instanceCount];
				for (int i = 0; i < instanceCount; i++)
				{
					InstanceHandles[i] = InstanceBuffer.Allocate(1);
					InstanceCount++;
				}
			}

			// A Model can contain multiple ModelParts, which in turn may contain multiple submeshes. Every submesh needs it's own instance.
			int instanceID = 0;
			foreach (ModelPart part in Model.Parts)
			{
				foreach (Mesh mesh in part.Meshes)
				{
					// Create material instance.
					MaterialInstances[instanceID] = new MaterialInstance(mesh.Material);

					// Make instance data.
					InstanceData instanceData = new()
					{
						MeshID = (uint)mesh.MeshHandle.ElementStart,
						TransformID = (uint)TransformHandle.ElementStart,
						MaterialID = (uint)MaterialInstances[instanceID].MaterialHandle.ElementStart,
					};

					// Upload instance to buffer.
					InstanceBuffer.SetData(InstanceHandles[instanceID], instanceData);
					instanceID++;
				}
			}

			IsInstanceDirty = false;
		}
	}
}