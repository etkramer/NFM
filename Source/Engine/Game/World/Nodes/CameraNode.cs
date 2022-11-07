using System;
using Engine.Rendering;
using Engine.Resources;

namespace Engine.World
{
	[Icon('\uE412')]
	public class CameraNode : Node
	{
		[Inspect]
		public uint FocalLength { get; set; } = 35;

		[Inspect]
		public uint SensorSize { get; set; } = 36;

		[Inspect]
		public float Exposure { get; set; } = 1;

		public float FOV => (2 * (float)Math.Atan(SensorSize / 2f / FocalLength)).ToDegrees();

		public CameraNode(Scene scene) : base(scene)
		{

		}

		public override void OnDrawGizmos(GizmosContext context)
		{

		}
	}
}