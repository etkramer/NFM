﻿using System;
using Engine.Rendering;
using Engine.Resources;

namespace Engine.World
{
	[Icon('\uE412')]
	public class CameraNode : Node
	{
		[Inspect] public uint FocalLength { get; set; } = 35;
		[Inspect] public uint SensorSize { get; set; } = 36;

		[Inspect] public float Exposure { get; set; } = 1;

		public float FOV => MathHelper.RadiansToDegrees(2 * (float)Math.Atan(SensorSize / 2f / FocalLength));

		public CameraNode()
		{

		}

		public override void OnDrawGizmos(GizmosContext context)
		{

		}
	}
}