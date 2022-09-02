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
		[Inspect] public bool IsVisible { get; set; } = true;
	
		// Mesh instances
		public bool IsInstanceDirty = true;
		public BufferAllocation<InstanceData>[] InstanceHandles;

		// Material instances
		public MaterialInstance[] MaterialInstances { get; set; }
		
		public ModelActor(string name = null) : base(name)
		{
			(this as INotify).Subscribe(nameof(Model), () => IsInstanceDirty = true);
			(this as INotify).Subscribe(nameof(IsVisible), () => IsInstanceDirty = true);
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
			// Get rid of existing instances.
			if (Model == null || Model?.Parts == null || !IsVisible)
			{
				for (int i = 0; i < InstanceHandles?.Length; i++)
				{
					MaterialInstances[i].Dispose();
					InstanceHandles[i].Dispose();
					InstanceCount--;
				}

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
				// Allocate a handful of new ones.
				MaterialInstances = new MaterialInstance[instanceCount];
				InstanceHandles = new BufferAllocation<InstanceData>[instanceCount];
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
						MeshID = (uint)mesh.MeshHandle.Start,
						TransformID = (uint)TransformHandle.Start,
						MaterialID = (uint)MaterialInstances[instanceID].MaterialHandle.Start,
					};

					// Upload instance to buffer.
					Renderer.DefaultCommandList.UploadBuffer(InstanceHandles[instanceID], instanceData);
					instanceID++;
				}
			}

			IsInstanceDirty = false;
		}
	}
}