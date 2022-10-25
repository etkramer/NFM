using System;
using Engine.GPU;

namespace Engine.World
{
	public partial class Scene
	{
		// Support up to 2^21 (~2M) objects in a scene.
		// This is a mostly arbitrary number chosen to be larger than most engines.
		public const int MaxInstances = 2097152;

		public GraphicsBuffer<GPUInstance> InstanceBuffer = new(MaxInstances);
		public GraphicsBuffer<GPUTransform> TransformBuffer = new(MaxInstances);
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
}
