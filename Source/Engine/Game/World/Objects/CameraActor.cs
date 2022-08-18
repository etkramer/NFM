﻿using System;

namespace Engine.World
{
	public sealed class CameraActor : Actor
	{
		[Inspect] public uint FocalLength { get; set; } = 35;
		[Inspect] public bool IsCinematic { get; set; }
	}
}