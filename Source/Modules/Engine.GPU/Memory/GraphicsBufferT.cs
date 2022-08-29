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

		public void SetData(BufferHandle<T> handle, T data) => SetData(handle.ElementStart, data);
		public void SetData(BufferHandle<T> handle, Span<T> data) => SetData(handle.ElementStart, data);

		public void SetData(long start, T data) => SetData(start, MemoryMarshal.CreateSpan(ref data, 1));
		public void SetData(long start, Span<T> data)
		{
			fixed (void* dataPtr = data)
			{
				SetData(dataPtr, data.Length * sizeof(T), start * sizeof(T));
			}
		}

		public T this[int i]
		{
			set
			{
				SetData(i, value);
			}
		}
	}
}
