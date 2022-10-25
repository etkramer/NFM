using System;
using Engine.GPU;
using Engine.GPU.Native;
using Engine.Rendering;
using Engine.Resources;

namespace Engine.World
{
	[Icon('\uE9FE')]
	public partial class ModelNode : Node
	{
		[Inspect] public Model Model { get; set; } = null;
		[Inspect] public bool IsVisible { get; set; } = true;

		/*[Inspect] */public Material[] Materials { get; set; } = null;
		/*[Inspect] */public List<Shader> Layers { get; set; } = new();
	
		// Mesh instances
		public bool IsInstanceDirty = true;
		public BufferAllocation<GPUInstance>[] InstanceHandles;

		// Material instances
		public MaterialInstance[] MaterialInstances { get; set; }
		
		public ModelNode()
		{
			(this as INotify).Subscribe(nameof(Model), () =>
			{
				IsInstanceDirty = true;
				Materials = Model.Parts.SelectMany(o => o.Meshes).Select(o => o.Material).ToArray();
			});

			(this as INotify).Subscribe(nameof(IsVisible), () => IsInstanceDirty = true);
		}

		public override void Dispose()
		{
			for (int i = 0; i < InstanceHandles?.Length; i++)
			{
				MaterialInstances[i].Dispose();

				// Zero out, then deallocate instance.
				Renderer.DefaultCommandList.UploadBuffer(InstanceHandles[i], default(GPUInstance));
				InstanceHandles[i].Dispose();
			}

			base.Dispose();
		}

		public void UpdateInstances()
		{
			// Get rid of existing instances.
			for (int i = 0; i < InstanceHandles?.Length; i++)
			{
				MaterialInstances[i].Dispose();

				// Zero out, then deallocate instance.
				Renderer.DefaultCommandList.UploadBuffer(InstanceHandles[i], default(GPUInstance));
				InstanceHandles[i].Dispose();
			}

			// If we don't have a fresh model to switch to, we're done here.
			if (Model == null || Model?.Parts == null || !Model.IsCommitted || !IsVisible || Scene == null)
			{
				InstanceHandles = null;
				MaterialInstances = null;
				return;
			}

			// Count instances.
			int instanceCount = Model.Parts.Sum(o => o.Meshes.Length);

			// (Re)build the array of instance handles.
			MaterialInstances = new MaterialInstance[instanceCount];
			InstanceHandles = new BufferAllocation<GPUInstance>[instanceCount];
			for (int i = 0; i < instanceCount; i++)
			{
				InstanceHandles[i] = Scene.InstanceBuffer.Allocate(1, true);
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
					GPUInstance instanceData = new()
					{
						MeshID = (int)mesh.MeshHandle.Offset,
						TransformID = (int)TransformHandle.Offset,
						MaterialID = (int)MaterialInstances[instanceID].MaterialHandle.Offset,
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