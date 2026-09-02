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

	/// <summary>
	/// Deformed vertices for each of this node's skinned meshes, written by the skinning pass.
	/// </summary>
	internal Dictionary<Mesh, RenderSkin> SkinHandles { get; } = [];

	internal BufferAllocation<Matrix4>? BoneHandle;

	private readonly Dictionary<Mesh, List<ShaderParameter>> materialOverrides = [];

	private Skeleton? spawnedSkeleton;
	private BoneNode[] bones = [];

	private RenderScene RenderScene => Scene.RenderData;

	public ModelNode(Scene? scene) : base(scene)
	{
		Name = "Model";

		TransformHandle = RenderScene.TransformBuffer.Allocate(1);

		// Track changes in model/visibility
		this.SubscribeFast(nameof(Model), RebuildSkeleton);
		this.SubscribeFast(nameof(Model), nameof(IsVisible), nameof(IsDetached), () => RenderScene.MarkInstancesDirty(this));
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

		if (IsVisible && !IsDetached && Model is not null)
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

			RenderScene.SetTransparent(InstanceHandles[mesh], this, mesh, material);

			// A skinned mesh draws from a deformed copy of its vertices, one per instance.
			nint vertexOffset = mesh.RenderData.VertexHandle.Offset;
			ulong blasAddress = mesh.RenderData.BLAS.GPUAddress;

			if (mesh.RenderData.WeightHandle is not null && BoneHandle is not null)
			{
				var skin = RenderSkin.Create(mesh.RenderData);

				SkinHandles[mesh] = skin;
				vertexOffset = skin.Vertices.Offset;
				blasAddress = skin.BLAS.GPUAddress;
			}

			// Upload instance to buffer
			list.UploadBuffer(InstanceHandles[mesh], new GPUInstance()
			{
				MeshID = (int)mesh.RenderData.MeshHandle.Offset,
				TransformID = (int)TransformHandle.Offset,
				MaterialID = (int)material.MaterialHandle.Offset,
				VertexOffset = (int)vertexOffset,
				Flags = material.InstanceFlags,
				BLASAddress = blasAddress,
			});
		}

		if (SkinHandles.Count > 0)
		{
			RenderScene.MarkDeformed(this);
		}
	}

	/// <summary>
	/// Replaces the owned bone tree with one matching the current model.
	/// </summary>
	private void RebuildSkeleton()
	{
		Model?.EnsureFullyLoaded();

		if (Model?.Skeleton == spawnedSkeleton)
		{
			return;
		}

		DespawnOwned();

		BoneHandle?.Dispose();
		BoneHandle = null;

		spawnedSkeleton = Model?.Skeleton;
		bones = [];

		if (spawnedSkeleton is not null)
		{
			bones = new BoneNode[spawnedSkeleton.Bones.Length];
			BoneHandle = RenderScene.BoneBuffer.Allocate(bones.Length);

			// Bones are ordered parents-first, so a parent is always spawned by the time it's needed.
			for (int i = 0; i < bones.Length; i++)
			{
				Bone bone = spawnedSkeleton.Bones[i];

				bones[i] = SpawnOwned(new BoneNode(Scene)
				{
					Name = bone.Name,
					Index = i,
					Position = bone.Position,
					Rotation = bone.Rotation,
					Scale = bone.Scale,
				}, bone.Name, bone.ParentIndex < 0 ? this : bones[bone.ParentIndex]);

				bones[i].SubscribeFast(nameof(WorldTransform), () => RenderScene.MarkBonesDirty(this));
			}

			RenderScene.MarkBonesDirty(this);
		}
	}

	/// <summary>
	/// Writes each bone's skinning matrix, which brings a bind-pose vertex to its posed position in
	/// model space.
	/// </summary>
	internal void UploadBones(CommandList list)
	{
		if (BoneHandle is null || spawnedSkeleton is null)
		{
			return;
		}

		Matrix4 worldToObject = WorldTransform.Inverse();
		Matrix4[] matrices = new Matrix4[bones.Length];

		for (int i = 0; i < bones.Length; i++)
		{
			matrices[i] = spawnedSkeleton.Bones[i].InverseBind * bones[i].WorldTransform * worldToObject;
		}

		list.UploadBuffer(BoneHandle, matrices);
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

		foreach (var skin in SkinHandles.Values)
		{
			skin.Dispose();
		}

		InstanceHandles.Clear();
		MaterialInstances.Clear();
		SkinHandles.Clear();

		RenderScene.DeformedNodes.Remove(this);
	}

	public override void Dispose()
	{
		RenderScene.Forget(this);

		ReleaseInstances(Renderer.DefaultCommandList);
		TransformHandle.Dispose();
		BoneHandle?.Dispose();

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

		context.DrawBox(modelBounds, Color.White);
	}
}
