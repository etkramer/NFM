using System;
using Engine.Rendering;
using Engine.Resources;

namespace Engine.World
{
	[Icon('\uE412')]
	public class CameraActor : Actor
	{
		[Inspect] public uint FocalLength { get; set; } = 35;
		[Inspect] public uint SensorSize { get; set; } = 36;

		[Inspect] public float Exposure { get; set; } = 1;

		public float FOV => MathHelper.RadiansToDegrees(2 * (float)Math.Atan(SensorSize / 2f / FocalLength));

		public CameraActor()
		{

		}

		public override void OnDrawGizmos(GizmosContext context)
		{
			//context.DrawLine(new Vector3(0), new Vector3(0, 0, 4));
			context.DrawLine(new Vector3(0), new Vector3(0, 0, 1), new Color(1));
		}
	}
}