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

	public TypedBuffer<GPUInstance> InstanceBuffer { get; } = new(MaxInstances) { Name = "Instance Buffer" };
	public TypedBuffer<GPUTransform> TransformBuffer { get; } = new(MaxInstances) { Name = "Transform Buffer" };

	private Dictionary<nint, ModelNode> instanceOwners = new();

	private HashSet<ModelNode> dirtyTransforms = new();
	private HashSet<ModelNode> dirtyInstances = new();

	public void MarkTransformDirty(ModelNode node) => dirtyTransforms.Add(node);
	public void MarkInstancesDirty(ModelNode node) => dirtyInstances.Add(node);

	public void Forget(ModelNode node)
	{
		dirtyTransforms.Remove(node);
		dirtyInstances.Remove(node);
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
}
