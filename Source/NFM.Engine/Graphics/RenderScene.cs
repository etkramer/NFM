using NFM.GPU;
using NFM.Resources;
using NFM.World;

namespace NFM.Graphics;

/// <summary>
/// GPU-side mirror of a <see cref="Scene"/>. Nodes mark themselves dirty as they're edited, and
/// every pending change is flushed to the GPU in one place by <see cref="Sync"/>.
/// </summary>
class RenderScene : IDisposable
{
	// Support up to 2^21 (~2M) objects in a scene.
	// This is a mostly arbitrary number chosen to be larger than most engines.
	public const int MaxInstances = 2097152;

	// Support up to 2^16 (65k) lights in a scene.
	public const int MaxLights = 65536;

	// Support up to 2^20 (~1M) posed bones in a scene.
	public const int MaxBones = 1048576;

	public TypedBuffer<GPUInstance> InstanceBuffer { get; } = new(MaxInstances) { Name = "Instance Buffer" };
	public TypedBuffer<GPUTransform> TransformBuffer { get; } = new(MaxInstances) { Name = "Transform Buffer" };
	public TypedBuffer<GPULight> LightBuffer { get; } = new(MaxLights) { Name = "Light Buffer" };
	public TypedBuffer<Matrix4> BoneBuffer { get; } = new(MaxBones) { Name = "Bone Buffer" };

	/// <summary>
	/// Everything traceable in the scene, rebuilt each frame by the BVH pass.
	/// </summary>
	public TopLevelAS TLAS { get; } = new();

	/// <summary>
	/// Nodes whose geometry has to be deformed this frame, and the structures over it refit. Drained
	/// by the BVH pass, once the skinning pass has run over the same set.
	/// </summary>
	public HashSet<ModelNode> DeformedNodes { get; } = [];

	private bool structuresDirty = true;

	private readonly Dictionary<nint, ModelNode> instanceOwners = [];

	private readonly HashSet<ModelNode> dirtyTransforms = [];
	private readonly HashSet<ModelNode> dirtyInstances = [];
	private readonly HashSet<ModelNode> dirtyBones = [];
	private readonly HashSet<LightNode> dirtyLights = [];

	/// <summary>
	/// Number of light slots a pass has to walk to cover every live light, including freed holes.
	/// </summary>
	public int LightCount => LightBuffer.NumAllocations > 0 ? (int)(LightBuffer.LastOffset + 1) : 0;

	/// <summary>
	/// Number of instance slots a pass has to walk to cover every live instance, including freed holes.
	/// </summary>
	public int InstanceCount => InstanceBuffer.NumAllocations > 0 ? (int)(InstanceBuffer.LastOffset + 1) : 0;

	public void MarkTransformDirty(ModelNode node)
	{
		dirtyTransforms.Add(node);
		MarkStructuresDirty();
	}

	public void MarkInstancesDirty(ModelNode node)
	{
		dirtyInstances.Add(node);
		MarkStructuresDirty();
	}

	public void MarkBonesDirty(ModelNode node)
	{
		dirtyBones.Add(node);
		MarkDeformed(node);
	}

	public void MarkLightDirty(LightNode node) => dirtyLights.Add(node);

	/// <summary>
	/// Queues a node's skinned geometry for another pass of deformation.
	/// </summary>
	public void MarkDeformed(ModelNode node)
	{
		DeformedNodes.Add(node);
		MarkStructuresDirty();
	}

	/// <summary>
	/// Flags the scene's structures as no longer matching its instances.
	/// </summary>
	public void MarkStructuresDirty() => structuresDirty = true;

	/// <summary>
	/// Reports whether the structures need rebuilding, clearing the flag so anything marked after
	/// this point carries over to the next frame.
	/// </summary>
	public bool TakeStructuresDirty()
	{
		bool result = structuresDirty;
		structuresDirty = false;

		return result;
	}

	public void Forget(ModelNode node)
	{
		dirtyTransforms.Remove(node);
		dirtyInstances.Remove(node);
		dirtyBones.Remove(node);
		DeformedNodes.Remove(node);

		MarkStructuresDirty();
	}

	public void Forget(LightNode node)
	{
		dirtyLights.Remove(node);
	}

	/// <summary>
	/// Flushes every pending node change to the GPU. Runs once per frame, on the main thread,
	/// and is the only place scene data is uploaded.
	/// </summary>
	public void Sync(CommandList list)
	{
		if (dirtyInstances.Count > 0)
		{
			foreach (var node in dirtyInstances)
			{
				node.RebuildInstances(list);
			}

			dirtyInstances.Clear();
		}

		if (dirtyTransforms.Count > 0)
		{
			foreach (var node in dirtyTransforms)
			{
				node.UploadTransform(list);
			}

			dirtyTransforms.Clear();
		}

		if (dirtyBones.Count > 0)
		{
			foreach (var node in dirtyBones)
			{
				node.UploadBones(list);
			}

			dirtyBones.Clear();
		}

		if (dirtyLights.Count > 0)
		{
			foreach (var node in dirtyLights)
			{
				node.UploadLight(list);
			}

			dirtyLights.Clear();
		}
	}

	/// <summary>
	/// Reserves an instance slot on behalf of a node. Allocating through the scene is what keeps
	/// <see cref="GetInstanceOwner"/> able to resolve the slot back to the node that owns it.
	/// </summary>
	public BufferAllocation<GPUInstance> AllocateInstance(ModelNode owner)
	{
		var handle = InstanceBuffer.Allocate(1, true);
		instanceOwners[handle.Offset] = owner;

		return handle;
	}

	public void FreeInstance(BufferAllocation<GPUInstance> handle)
	{
		instanceOwners.Remove(handle.Offset);
		transparentInstances.Remove(handle.Offset);
		handle.Dispose();
	}

	/// <summary>Blended instances, which the forward pass draws in sorted order instead of the visbuffer.</summary>
	public IReadOnlyCollection<TransparentInstance> TransparentInstances => transparentInstances.Values;
	private readonly Dictionary<nint, TransparentInstance> transparentInstances = [];

	/// <summary>Claims an instance slot for the forward pass, or releases it to the visbuffer when opaque.</summary>
	public void SetTransparent(BufferAllocation<GPUInstance> handle, ModelNode owner, Mesh mesh, RenderMaterial material)
	{
		if (material.BlendMode is BlendMode.Opaque or BlendMode.Masked)
		{
			transparentInstances.Remove(handle.Offset);
			return;
		}

		transparentInstances[handle.Offset] = new TransparentInstance()
		{
			InstanceID = (int)handle.Offset,
			Owner = owner,
			Mesh = mesh,
			Material = material,
		};
	}

	/// <summary>
	/// Finds the node that owns a given slot in the instance buffer, as read back from the visbuffer.
	/// </summary>
	public ModelNode? GetInstanceOwner(int instanceID)
	{
		return instanceOwners.GetValueOrDefault(instanceID);
	}

	public void Dispose()
	{
		InstanceBuffer.Dispose();
		TransformBuffer.Dispose();
		LightBuffer.Dispose();
		BoneBuffer.Dispose();
		TLAS.Dispose();
	}
}

public struct GPUTransform
{
	public Matrix4 ObjectToWorld;
	public Matrix4 WorldToObject;
}

/// <summary>One blended instance, as drawn by the forward pass.</summary>
readonly struct TransparentInstance
{
	public required int InstanceID { get; init; }
	public required ModelNode Owner { get; init; }
	public required Mesh Mesh { get; init; }
	public required RenderMaterial Material { get; init; }

	/// <summary>World-space bounds centre, which the pass sorts back to front on.</summary>
	public Vector3 SortOrigin => (new Vector4(Mesh.Bounds.Center, 1) * Owner.WorldTransform).Xyz;
}

[Flags]
public enum InstanceFlags : uint
{
	BlendMask = 0x3,

	BlendOpaque = 0,
	BlendMasked = 1,
	BlendOver = 2,
	BlendAdditive = 3,
}

/// <summary>The blend modes a culling dispatch draws, as a mask of 1 &lt;&lt; blend mode.</summary>
[Flags]
public enum CullBuckets
{
	Visbuffer = (1 << (int)InstanceFlags.BlendOpaque) | (1 << (int)InstanceFlags.BlendMasked),
	Transparent = (1 << (int)InstanceFlags.BlendOver) | (1 << (int)InstanceFlags.BlendAdditive),
}

[StructLayout(LayoutKind.Sequential)]
public struct GPUInstance
{
	public int MeshID;
	public int MaterialID;
	public int TransformID;
	public int VertexOffset; // Start of this instance's vertices, deformed or shared with the mesh
	public InstanceFlags Flags;
	public uint Pad; // Keeps BLASAddress 8-byte aligned, matching the shader's uint2
	public ulong BLASAddress; // Structure to trace against, deformed or shared with the mesh
}

[StructLayout(LayoutKind.Sequential)]
public struct GPULight
{
	public const uint None = 0;
	public const uint Point = 1;

	public uint Type;
	public Vector3 Position;
	public Vector3 Color; // Linear RGB, scaled by intensity in candela
	public float Radius; // Source radius, in meters
}
