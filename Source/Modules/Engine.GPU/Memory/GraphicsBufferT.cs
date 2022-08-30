using System;
using System.Collections.Concurrent;
using Vortice.Direct3D12;

namespace Engine.GPU
{
	public unsafe class GraphicsBuffer<T> : GraphicsBuffer, IDisposable where T : unmanaged
	{
		private List<Block> blocks = new();

		public struct Block
		{
			public long Start;
			public long Length;

			public Block(long start, long length)
			{
				Start = start;
				Length = length;
			}
		}

		public GraphicsBuffer(int elementCount, int alignment = 1, bool hasCounter = false, bool isRaw = false) : base(elementCount * sizeof(T), sizeof(T), alignment, hasCounter, isRaw)
		{

		}

		/// <summary>
		/// Allocates space in the buffer and returns a handle.
		/// </summary>
		public BufferAllocation<T> Allocate(int count)
		{
			lock (blocks)
			{
				Block block = default;
				for (long i = 0; i < Capacity; i++)
				{
					long goalStart = i;
					long goalEnd = i + count;

					bool blocked = false;
					for (int j = 0; j < blocks.Count; j++) // Loop through blocks that might obstruct this area.
					{
						long blockStart = blocks[j].Start - 1;
						long blockEnd = blockStart + blocks[j].Length;

						// Can already tell this block isn't in the way.
						if (blockStart > goalEnd || blockEnd < goalStart)
						{
							continue;
						}

						// Check if the goal area is obstructed by this block.
						blocked = (goalStart >= blockStart && goalStart <= blockEnd) // Starts inside block
							|| (goalEnd >= blockStart && goalEnd <= blockEnd) // Ends inside block
							|| (goalStart <= blockStart && goalEnd >= blockStart) // Overlaps block start
							|| (goalStart <= blockEnd && goalEnd >= blockEnd); // Overlaps block end

						// There's a block in the way, and we know where it ends. Skip past known blocked elements.
						if (blocked)
						{
							i = blockEnd;
							break;
						}
					}

					if (!blocked)
					{
						block = new Block()
						{
							Start = goalStart,
							Length = count
						};

						blocks.Add(block);
						break;
					}
				}

				// Couldn't find a large enough block.
				if (block.Length == 0)
				{
					Resize(Capacity * 2);
					return Allocate(count);
				}
				
				BufferAllocation<T> handle = new(this)
				{
					ElementStart = block.Start,
					ElementCount = block.Length,
				};

				return handle;
			}
		}

		public void Resize(long newCapacity)
		{
			throw new NotImplementedException("Buffer resizing not yet supported");
		}

		public void Free(BufferAllocation<T> handle)
		{
			lock (blocks)
			{
				blocks.RemoveAt(blocks.FindIndex(o => o.Start == handle.ElementStart && o.Length == handle.ElementCount));
			}
		}

		public void Clear()
		{
			lock (blocks)
			{
				blocks.Clear();
			}
		}
	}

	public class BufferAllocation<T> : IDisposable where T : unmanaged
	{
		public long ElementStart = 0;
		public long ElementCount = 0;
		public GraphicsBuffer<T> Buffer { get; private set; }

		public BufferAllocation(GraphicsBuffer<T> source)
		{
			Buffer = source;
		}

		public void Dispose()
		{
			Buffer.Free(this);
		}
	}
}