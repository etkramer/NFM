using System.Diagnostics.CodeAnalysis;
using NFM.GPU;
using NFM.Graphics;
using NFM.Resources;

namespace NFM.World;

[Icon("view_in_ar")]
public partial class ModelNode : Node
{
	[Inspect]
	public Model? Model { get; set; } = null;

	[Inspect]
	public bool IsVisible { get; set; } = true;

	internal BufferAllocation<GPUTransform> TransformHandle;

	internal Dictionary<Mesh, BufferAllocation<GPUInstance>> InstanceHandles { get; } = new();
	internal Dictionary<Mesh, RenderMaterial> MaterialInstances { get; } = new();
	
	public ModelNode(Scene? scene) : base(scene)
	{
		Name = "Model";

		// Track changes in model/visibility
		this.SubscribeFast(nameof(Model), nameof(IsVisible), () =>
		{
            Model?.EnsureFullyLoaded();
			UpdateInstances(Renderer.DefaultCommandList);
		});

		this.SubscribeFast(nameof(WorldTransform), UpdateTransform);
		UpdateTransform();
	}

    [MemberNotNull(nameof(TransformHandle))]
    void UpdateTransform()
    {
		TransformHandle ??= Scene.TransformBuffer.Allocate(1);
		Renderer.DefaultCommandList.UploadBuffer(TransformHandle, new GPUTransform()
		{
			ObjectToWorld = WorldTransform,
			WorldToObject = WorldTransform.Inverse()
		});
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

		if (!IsVisible || Model is null)
		{
			return;
		}

        // (Re)build the array of instance handles
        foreach (var group in Model.MeshGroups)
        {
            // Don't show hidden mesh groups. TODO: override on a per-ModelNode basis.
            if (!group.IsVisible)
            {
                continue;
            }

            foreach (var mesh in group.Meshes)
            {
                // Only show LOD0 for now.
                if ((mesh.LODMask & LODLevel.LOD0) == 0)
                {
                    continue;
                }

                Guard.NotNull(mesh.RenderData);

				InstanceHandles[mesh] = Scene.InstanceBuffer.Allocate(1, true);
				MaterialInstances[mesh] = new RenderMaterial(Guard.NotNull(mesh.Material));

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

	public void OnDrawGizmos(object? sender, Gizmos context)
	{
        if (Model is null || !IsVisible)
        {
            return;
        }

        // Combine mesh bounds to represent model.
        Box3D modelBounds = Model.Meshes
            .Select(mesh => mesh.Bounds)
            .Aggregate((a, c) => a + c);

        // Bring bounds into world space.
        modelBounds.Min = (new Vector4(modelBounds.Min, 1) * WorldTransform).Xyz;
        modelBounds.Max = (new Vector4(modelBounds.Max, 1) * WorldTransform).Xyz;

		context.DrawBox(modelBounds, Color.White, Color.Invisible);
	}
}