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

		public float FOV => MathHelper.RadiansToDegrees(2 * (float)Math.Atan(SensorSize / 2f / FocalLength));

		public CameraActor(string name = null) : base(name)
		{

		}
	}
}