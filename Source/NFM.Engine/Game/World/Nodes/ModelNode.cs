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

	internal Dictionary<Mesh, BufferAllocation<GPUInstance>> InstanceHandles { get; } = [];
	internal Dictionary<Mesh, RenderMaterial> MaterialInstances { get; } = [];

	private readonly Dictionary<Mesh, List<ShaderParameter>> materialOverrides = [];

	private RenderScene RenderScene => Scene.RenderData;

	public ModelNode(Scene? scene) : base(scene)
	{
		Name = "Model";

		TransformHandle = RenderScene.TransformBuffer.Allocate(1);

		// Track changes in model/visibility
		this.SubscribeFast(nameof(Model), nameof(IsVisible), () => RenderScene.MarkInstancesDirty(this));
		this.SubscribeFast(nameof(WorldTransform), () => RenderScene.MarkTransformDirty(this));

		RenderScene.MarkInstancesDirty(this);
		RenderScene.MarkTransformDirty(this);
	}

	/// <summary>
	/// Overrides a shader parameter for one of this node's meshes, on top of the mesh's own material.
	/// </summary>
	public void SetMaterialOverride(Mesh mesh, string param, object? value)
	{
		if (!materialOverrides.TryGetValue(mesh, out var overrides))
		{
			overrides = materialOverrides[mesh] = [];
		}

		overrides.RemoveAll(o => o.Name == param);
		overrides.Add(new ShaderParameter()
		{
			Name = param,
			Value = value,
			Type = value?.GetType() ?? typeof(object)
		});

		RenderScene.MarkInstancesDirty(this);
	}

	public void ClearMaterialOverrides(Mesh mesh)
	{
		if (materialOverrides.Remove(mesh))
		{
			RenderScene.MarkInstancesDirty(this);
		}
	}

	internal void UploadTransform(CommandList list)
	{
		list.UploadBuffer(TransformHandle, new GPUTransform()
		{
			ObjectToWorld = WorldTransform,
			WorldToObject = WorldTransform.Inverse()
		});
	}

	internal void RebuildInstances(CommandList list)
	{
		Model?.EnsureFullyLoaded();

		// Acquire the new materials before releasing the old ones, so anything shared between the
		// two isn't dropped to zero references and immediately rebuilt.
		Dictionary<Mesh, RenderMaterial> newMaterials = [];

		if (IsVisible && Model is not null)
		{
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

					newMaterials[mesh] = RenderMaterial.Get(Guard.NotNull(mesh.Material), materialOverrides.GetValueOrDefault(mesh));
				}
			}
		}

		ReleaseInstances(list);

		// (Re)build the array of instance handles
		foreach (var (mesh, material) in newMaterials)
		{
			Guard.NotNull(mesh.RenderData);

			InstanceHandles[mesh] = RenderScene.AllocateInstance(this);
			MaterialInstances[mesh] = material;

			// Upload instance to buffer
			list.UploadBuffer(InstanceHandles[mesh], new GPUInstance()
			{
				MeshID = (int)mesh.RenderData.MeshHandle.Offset,
				TransformID = (int)TransformHandle.Offset,
				MaterialID = (int)material.MaterialHandle.Offset,
			});
		}
	}

	private void ReleaseInstances(CommandList list)
	{
		foreach (var mesh in InstanceHandles.Keys)
		{
			// Zero out instance data
			list.UploadBuffer(InstanceHandles[mesh], default(GPUInstance));

			RenderScene.FreeInstance(InstanceHandles[mesh]);
			MaterialInstances[mesh].Dispose();
		}

		InstanceHandles.Clear();
		MaterialInstances.Clear();
	}

	public override void Dispose()
	{
		RenderScene.Forget(this);

		ReleaseInstances(Renderer.DefaultCommandList);
		TransformHandle.Dispose();

		base.Dispose();
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
