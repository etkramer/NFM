using System;
using Vortice.Direct3D12;

namespace Engine.GPU
{
	public unsafe partial class GraphicsBuffer<T>
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

		/// <summary>
		/// Allocates space in the buffer and returns a handle.
		/// </summary>
		public BufferHandle<T> Allocate(int count)
		{
			lock (blocks)
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

					// Is there any empty space?
					if (blocks[i].Length - blockA.Length > 0)
					{
						// Make a new block to represent it.
						Block blockB = new()
						{
							Start = blockA.Start + blockA.Length,
							Length = blocks[i].Length - blockA.Length,
							Free = true,
						};

						blocks.Insert(i + 1, blockB);
					}

					blocks[i] = blockA;
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
		}

		public void Resize(long newCapacity)
		{
			throw new NotImplementedException("Buffer resizing not yet supported");
		}

		public void Free(BufferHandle<T> handle)
		{
			lock (blocks)
			{
				// Invalid, probably uninitialized handle.
				if (handle.ElementStart == 0 && handle.ElementCount == 0)
				{
					return;
				}

				for (int i = 0; i < blocks.Count; i++)
				{
					// Found the block.
					if (blocks[i].Start == handle.ElementStart && blocks[i].Length == handle.ElementCount)
					{
						Block freeBlock = blocks[i];
						freeBlock.Free = true;

						blocks[i] = freeBlock;
					}
				}
			}
		}

		public void FreeAll()
		{
			lock (blocks)
			{
				blocks.Clear();
				blocks.Add
				(
					new Block() { Start = 0, Length = Capacity, Free = true }
				);
			}
		}
	}

	public class BufferHandle<T> : IDisposable where T : unmanaged
	{
		public long ElementStart = 0;
		public long ElementCount = 0;
		private GraphicsBuffer<T> buffer;

		public BufferHandle(GraphicsBuffer<T> source)
		{
			buffer = source;
		}

		public void Dispose()
		{
			buffer.Free(this);
		}
	}
}