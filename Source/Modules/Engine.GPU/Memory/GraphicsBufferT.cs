using System;
using System.Runtime.InteropServices;

namespace Engine.GPU
{
	public unsafe partial class GraphicsBuffer<T> : GraphicsBuffer, IDisposable where T : unmanaged
	{
		public GraphicsBuffer(int capacity, int alignment = 1) : base(capacity * sizeof(T), sizeof(T), alignment)
		{
			// Create initial allocation block.
			blocks.Add(new Block()
			{
				Start = 0,
				Length = Capacity,
				Free = true,
			});
		}

		/// <summary>
		/// Allocates space for the data, then uploads it to the GPU.
		/// </summary>
		public BufferHandle<T> Upload(T[] data)
		{
			var handle = Allocate(data.Length);
			SetData(handle, data);
			return handle;
		}

		/// <inheritdoc cref="Upload(T[])"/>
		public BufferHandle<T> Upload(T data)
		{
			var handle = Allocate(1);
			SetData(handle, data);
			return handle;
		}

		/// <inheritdoc cref="Upload(T[])"/>
		public BufferHandle<T> Upload(Span<T> data)
		{
			var handle = Allocate(data.Length);
			SetData(handle, data);
			return handle;
		}

		public void SetData(BufferHandle<T> handle, T[] data) => SetData(handle.ElementStart, data);
		public void SetData(BufferHandle<T> handle, T data) => SetData(handle.ElementStart, data);
		public void SetData(BufferHandle<T> handle, Span<T> data) => SetData(handle.ElementStart, data);

		public void SetData(long start, T[] data) => SetData(start, data.AsSpan());
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
