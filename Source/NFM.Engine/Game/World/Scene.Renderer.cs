using NFM.GPU;

namespace NFM.World;

public partial class Scene
{
	// Support up to 2^21 (~2M) objects in a scene.
	// This is a mostly arbitrary number chosen to be larger than most engines.
	public const int MaxInstances = 2097152;

	public TypedBuffer<GPUInstance> InstanceBuffer = new(MaxInstances);
	public TypedBuffer<GPUTransform> TransformBuffer = new(MaxInstances);

	private Dictionary<nint, ModelNode> instanceOwners = new();

	/// <summary>
	/// Reserves an instance slot on behalf of a node. Allocating through the scene is what keeps
	/// <see cref="GetInstanceOwner"/> able to resolve the slot back to the node that owns it.
	/// </summary>
	internal BufferAllocation<GPUInstance> AllocateInstance(ModelNode owner)
	{
		var handle = InstanceBuffer.Allocate(1, true);
		instanceOwners[handle.Offset] = owner;

		return handle;
	}

	internal void FreeInstance(BufferAllocation<GPUInstance> handle)
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
