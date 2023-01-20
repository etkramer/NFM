using System;
using System.Collections.Generic;
using Avalonia;
using NFM.Editor;
using NFM.GPU;
using NFM.Rendering;
using NFM.Resources;
using ReactiveUI;

namespace NFM.World
{
	[Icon('\uE9FE')]
	public partial class ModelNode : Node
	{
		[Inspect]
		public Model Model { get; set; } = null;

		[Inspect]
		public bool IsVisible { get; set; } = true;
	
		// Mesh instances
		public BufferAllocation<GPUTransform> TransformHandle;
		public BufferAllocation<GPUInstance>[] InstanceHandles;

		// Material instances
		public MaterialInstance[] MaterialInstances { get; set; }
		
		public ModelNode(Scene scene) : base(scene)
		{
			Name = "Model";

			// Track changes in model/visibility
			this.SubscribeFast(nameof(Model), nameof(IsVisible), () =>
			{
				UpdateInstances(Renderer.DefaultCommandList);
			});

			this.SubscribeFast(nameof(WorldTransform), () =>
			{
				TransformHandle ??= Scene.TransformBuffer.Allocate(1);
				Renderer.DefaultCommandList.UploadBuffer(TransformHandle, new GPUTransform()
				{
					ObjectToWorld = WorldTransform,
					WorldToObject = WorldTransform.Inverse()
				});
			});
		}

		public override void Dispose()
		{
			TransformHandle?.Dispose();

			for (int i = 0; i < InstanceHandles?.Length; i++)
			{
				MaterialInstances[i].Dispose();

				// Zero out, then deallocate instance.
				Renderer.DefaultCommandList.UploadBuffer(InstanceHandles[i], default(GPUInstance));
				InstanceHandles[i].Dispose();
			}

			base.Dispose();
		}

		public void UpdateInstances(CommandList list)
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
					list.UploadBuffer(InstanceHandles[instanceID], instanceData);
					instanceID++;
				}
			}
		}

		public override void DrawGizmos(GizmosContext context)
		{
			if (Selection.Selected.Contains(this) && Model != null && IsVisible)
			{
				Box3D modelBounds = Box3D.Zero;
				foreach (var part in Model.Parts)
				{
					foreach (var mesh in part.Meshes)
					{
						modelBounds += mesh.Bounds;
					}
				}

				context.DrawBox(modelBounds, Color.White, Color.Invisible);
			}

			base.DrawGizmos(context);
		}
	}
}