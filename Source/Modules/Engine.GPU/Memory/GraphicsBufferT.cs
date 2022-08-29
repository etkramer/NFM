using System;
using System.Runtime.InteropServices;

namespace Engine.GPU
{
	public unsafe partial class GraphicsBuffer<T> : GraphicsBuffer, IDisposable where T : unmanaged
	{
		public GraphicsBuffer(int elementCount, int alignment = 1, bool hasCounter = false, bool isRaw = false) : base(elementCount * sizeof(T), sizeof(T), alignment, hasCounter, isRaw)
		{
			// Create initial allocation block.
			blocks.Add(new Block()
			{
				Start = 0,
				Length = Capacity,
				Free = true,
			});
		}
	}
}
