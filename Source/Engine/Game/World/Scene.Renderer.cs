using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Engine.GPU;

namespace Engine.World
{
	public partial class Scene
	{
		public const int MaxInstances = 2048^21; // Support up to ~2M objects in a scene. This is around the same as Unreal, and several times smaller than CryEngine.

		public int InstanceCount { get; set; } = 0;
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
		public uint MeshID;
		public uint MaterialID;
		public uint TransformID;
	}
}
