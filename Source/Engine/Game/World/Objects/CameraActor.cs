using System;

namespace Engine.World
{
	public class CameraActor : Actor
	{
		[Inspect] public uint FocalLength { get; set; } = 35;
		[Inspect] public uint SensorSize { get; set; } = 36;

		public CameraActor(string name = null) : base(name)
		{

		}

		/// <summary>
		/// Calculates the vertical field of view from physical camera properties.
		/// </summary>
		/// <returns></returns>
		public float CalcFOV()
		{
			return MathHelper.RadiansToDegrees(2 * (float)Math.Atan(SensorSize / 2f /FocalLength));
		}
	}
}