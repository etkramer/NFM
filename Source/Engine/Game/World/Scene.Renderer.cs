using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Engine.GPU;

namespace Engine.World
{
	public partial class Scene
	{
		public const int MaxInstanceCount = 100;

		public int InstanceCount { get; set; } = 0;
		public GraphicsBuffer<GPUInstance> InstanceBuffer = new(MaxInstanceCount);
		public GraphicsBuffer<GPUTransform> TransformBuffer = new(MaxInstanceCount);
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
