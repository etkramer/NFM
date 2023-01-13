using System;
using System.Collections.Concurrent;
using NFM.GPU.Native;
using Vortice.Direct3D12;

namespace NFM.GPU
{
	public unsafe class GraphicsBuffer<T> : GraphicsBuffer, IDisposable where T : unmanaged
	{
		public nint NumAllocations { get; private set; } = 0;
		public nint FirstOffset { get; private set; } = 0;
		public nint LastOffset { get; private set; } = 0;

		private D3D12MA.VirtualBlock virtualBlock;
		private List<BufferAllocation<T>> allocations = new();

		public GraphicsBuffer(nint elementCount, int alignment = 1, bool hasCounter = false, bool isRaw = false) : base(elementCount * sizeof(T), sizeof(T), alignment, hasCounter, isRaw)
		{
			// Create block with D3D12MA
			D3D12MA.CreateVirtualBlock(new D3D12MA.VirtualBlockDescription()
			{
				Size = (ulong)elementCount,
				Flags = D3D12MA.VirtualBlockFlags.None,
			}, out virtualBlock);
		}

		public override void Dispose()
		{
			base.Dispose();
			virtualBlock.Release();
		}

		/// <summary>
		/// Allocates space in the buffer and returns a handle.
		/// </summary>
		public BufferAllocation<T> Allocate(nint count, bool preferMinOffset = false)
		{
			lock (virtualBlock)
			{
				var flags = D3D12MA.VirtualAllocationFlags.None;
				if (preferMinOffset)
				{
					flags |= D3D12MA.VirtualAllocationFlags.MinOffset;
				}

				// Allocate space with D3D12MA
				Debug.Assert(virtualBlock.Allocate(new D3D12MA.VirtualAllocationDescription()
				{
					Size = (ulong)count,
					Alignment = 0,
					Flags = flags
				}, out var allocation, out _).Success, "Failed to allocate space from buffer");

				// Create a handle from the D3D12MA allocation.
				virtualBlock.GetAllocationInfo(allocation, out var info);
				var alloc = new BufferAllocation<T>(this)
				{
					Handle = allocation,
					Offset = (nint)info.Offset,
					Size = (nint)info.Size
				};

				allocations.Add(alloc);
				UpdateStats();

				return alloc;
			}
		}

		public void Free(BufferAllocation<T> alloc)
		{
			lock (virtualBlock)
			{
				virtualBlock.FreeAllocation(alloc.Handle);

				allocations.Remove(alloc);
				UpdateStats();
			}
		}

		private void UpdateStats()
		{
			NumAllocations = allocations.Count;

			if (allocations.Count == 0)
			{
				FirstOffset = 0;
				LastOffset = 0;
			}
			else
			{
				FirstOffset = allocations.Min(o => (int)o.Offset);
				LastOffset = allocations.Max(o => (int)o.Offset);
			}
		}

		public void Clear()
		{
			lock (virtualBlock)
			{
				virtualBlock.Clear();
			}
		}
	}

	public class BufferAllocation<T> : IDisposable where T : unmanaged
	{
		public nint Offset = 0;
		public nint Size = 0;
		public nint End => Offset + Size;
		
		internal D3D12MA.VirtualAllocation Handle;
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