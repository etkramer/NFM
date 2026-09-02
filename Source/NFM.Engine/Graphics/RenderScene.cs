using NFM.GPU;
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
	/// Every node with geometry to deform this frame, walked by the skinning pass.
	/// </summary>
	public HashSet<ModelNode> SkinnedNodes { get; } = [];

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

	public void MarkTransformDirty(ModelNode node) => dirtyTransforms.Add(node);
	public void MarkInstancesDirty(ModelNode node) => dirtyInstances.Add(node);
	public void MarkBonesDirty(ModelNode node) => dirtyBones.Add(node);
	public void MarkLightDirty(LightNode node) => dirtyLights.Add(node);

	public void Forget(ModelNode node)
	{
		dirtyTransforms.Remove(node);
		dirtyInstances.Remove(node);
		dirtyBones.Remove(node);
		SkinnedNodes.Remove(node);
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
		handle.Dispose();
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

[StructLayout(LayoutKind.Sequential)]
public struct GPUInstance
{
	public int MeshID;
	public int MaterialID;
	public int TransformID;
	public int VertexOffset; // Start of this instance's vertices, deformed or shared with the mesh
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
