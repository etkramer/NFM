using System;
using System.Collections;
using System.Collections.Generic;
using NFM.GPU;
using NFM.Graphics;
using NFM.Resources;
using ReactiveUI;

namespace NFM.World;

[Icon('\uE9FE')]
public partial class ModelNode : Node
{
	[Inspect]
	public Model Model { get; set; } = null;

	[Inspect]
	public bool IsVisible { get; set; } = true;

	// Transforms
	internal BufferAllocation<GPUTransform> TransformHandle;

	// Mesh data
	internal Dictionary<Mesh, BufferAllocation<GPUInstance>> InstanceHandles { get; } = new();
	internal Dictionary<Mesh, RenderMaterial> MaterialInstances { get; } = new();
	
	public ModelNode(Scene scene) : base(scene)
	{
		Name = "Model";

		// Track changes in model/visibility
		this.SubscribeFast(nameof(Model), nameof(IsVisible), () =>
		{
			UpdateInstances(Renderer.DefaultCommandList);
		});

		Action updateTransform = () =>
		{
			TransformHandle ??= Scene.TransformBuffer.Allocate(1);
			Renderer.DefaultCommandList.UploadBuffer(TransformHandle, new GPUTransform()
			{
				ObjectToWorld = WorldTransform,
				WorldToObject = WorldTransform.Inverse()
			});
		};

		this.SubscribeFast(nameof(WorldTransform), updateTransform);
		updateTransform.Invoke();
	}

	public override void Dispose()
	{
		TransformHandle?.Dispose();

		foreach (var mesh in InstanceHandles.Keys)
		{
			MaterialInstances[mesh].Dispose();

			Renderer.DefaultCommandList.UploadBuffer(InstanceHandles[mesh], default(GPUInstance));
			InstanceHandles[mesh].Dispose();
		}

		base.Dispose();
	}

	void UpdateInstances(CommandList list)
	{
		// Clear existing instances
		foreach (var mesh in InstanceHandles.Keys)
		{
			// Zero out instance data
			Renderer.DefaultCommandList.UploadBuffer(InstanceHandles[mesh], default(GPUInstance));

			InstanceHandles[mesh].Dispose();
			MaterialInstances[mesh].Dispose();
		}

		InstanceHandles.Clear();
		MaterialInstances.Clear();

		if (!IsVisible || Model == null)
		{
			return;
		}

		// (Re)build the array of instance handles
		foreach (var mesh in Model.Meshes)
		{
			if (mesh.IsVisible)
			{
				InstanceHandles[mesh] = Scene.InstanceBuffer.Allocate(1, true);
				MaterialInstances[mesh] = new RenderMaterial(mesh.Materials[0]);

				// Build instance data
				GPUInstance instanceData = new()
				{
					MeshID = (int)mesh.RenderData.MeshHandle.Offset,
					TransformID = (int)TransformHandle.Offset,
					MaterialID = (int)MaterialInstances[mesh].MaterialHandle.Offset,
				};

				// Upload instance to buffer
				list.UploadBuffer(InstanceHandles[mesh], instanceData);
			}
		}
	}

	public override void OnSelect()
	{
		Gizmos.OnDrawGizmos += OnDrawGizmos;
		base.OnSelect();
	}

	public override void OnDeselect()
	{
		Gizmos.OnDrawGizmos -= OnDrawGizmos;
		base.OnDeselect();
	}

	public void OnDrawGizmos(object sender, Gizmos context)
	{
		if (Model != null && IsVisible)
		{
			Box3D modelBounds = Box3D.Zero;
			foreach (var mesh in Model.Meshes)
			{
				if (mesh.IsVisible)
				{
					modelBounds += mesh.Bounds;
				}
			}

			context.DrawBox(modelBounds, Color.White, Color.Invisible);
		}
	}
}