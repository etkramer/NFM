using System;
using Vortice.DXGI;
using Vortice.Direct3D12;
using System.Runtime.InteropServices;
using Engine.Aspects;

namespace Engine.GPU
{
	public unsafe partial class GraphicsBuffer<T> where T : unmanaged
	{
		private List<Block> blocks = new();

		public struct Block
		{
			public long Start;
			public long Length;
			public bool Free;

			public Block(long start, long length, bool free)
			{
				Start = start;
				Free = free;
				Length = length;
			}
		}

		public BufferHandle<T> Allocate(int count)
		{
			int blockIndex = -1;

			// Loop through all blocks.
			for (int i = 0; i < blocks.Count; i++)
			{
				// Don't overwrite occupied blocks.
				if (!blocks[i].Free)
					continue;

				// Block doesn't have enough space
				if (blocks[i].Length < count)
					continue;

				// Put the new data in this block
				Block blockA = new()
				{
					Start = blocks[i].Start,
					Length = count,
					Free = false
				};

				// And make a new one to represent the empty space.
				Block blockB = new()
				{
					Start = blockA.Start + blockA.Length,
					Length = blocks[i].Length - blockA.Length,
					Free = true,
				};

				// Remove existing block.
				blocks.RemoveAt(i);

				// Create new blocks
				blocks.Insert(i, blockA);
				blocks.Insert(i + 1, blockB);
				blockIndex = i;
				break;
			}

			// Couldn't find a large enough block.
			if (blockIndex == -1)
			{
				Resize(Capacity * 2);
				return Allocate(count);
			}

			BufferHandle<T> handle = new(this)
			{
				ElementStart = blocks[blockIndex].Start,
				ElementCount = blocks[blockIndex].Length,
			};

			return handle;
		}

		public void Resize(long newCapacity)
		{
			throw new NotImplementedException("Buffer resizing not yet supported");
		}

		public void Free(BufferHandle<T> handle)
		{
			// Invalid, probably uninitialized handle.
			if (handle == default)
			{
				return;
			}

			for (int i = 0; i < blocks.Count; i++)
			{
				if (blocks[i].Start == handle.ElementStart && blocks[i].Length == handle.ElementCount)
				{
					if (i > 0)
					{
						// Previous block is free.
						if (blocks[i - 1].Free)
						{
							Block extendedBlock = blocks[i - 1];
							extendedBlock.Length += blocks[i].Length;
						}
					}
					if (blocks.Count > i)
					{
						// Next block is free.
						if (blocks[i + 1].Free)
						{
							Block extendedBlock = blocks[i + 1];
							extendedBlock.Length += blocks[i].Length;
							extendedBlock.Start -= blocks[i].Length;
						}
					}
					else
					{
						// Neighbors are used, just mark this one as free.
						Block freeBlock = blocks[i];
						freeBlock.Free = true;

						blocks[i] = freeBlock;
					}
				}
			}
		}

		public void FreeAll()
		{
			blocks.Clear();
			blocks.Add
			(
				new Block() { Start = 0, Length = Capacity, Free = true }
			);
		}
	}

	[AutoDispose]
	public class BufferHandle<T> : IDisposable where T : unmanaged
	{
		public long ElementStart;
		public long ElementCount;

		private GraphicsBuffer<T> buffer;

		public BufferHandle(GraphicsBuffer<T> source)
		{
			buffer = source;
		}

		public void Free()
		{
			buffer.Free(this);
		}

		public void Dispose() => Free();
	}
}