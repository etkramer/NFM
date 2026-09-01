using NFM.GPU.Native;

namespace NFM.GPU;

public unsafe class TypedBuffer<T> : RawBuffer, IDisposable where T : unmanaged
{
	public nint NumAllocations => allocations.Count;
	public nint FirstOffset { get { RefreshStats(); return firstOffset; } }
	public nint LastOffset { get { RefreshStats(); return lastOffset; } }

	private nint firstOffset = 0;
	private nint lastOffset = 0;
	private bool statsDirty = false;

	private readonly D3D12MA.VirtualBlock virtualBlock;
	private readonly List<BufferAllocation<T>> allocations = [];

	public TypedBuffer(nint elementCount, int alignment = 1, bool hasCounter = false, bool isRaw = false) : base(elementCount * sizeof(T), sizeof(T), alignment, hasCounter, isRaw)
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
		var flags = D3D12MA.VirtualAllocationFlags.None;
		if (preferMinOffset)
		{
			flags |= D3D12MA.VirtualAllocationFlags.MinOffset;
		}

		// Allocate space with D3D12MA
		Guard.Require(virtualBlock.Allocate(new D3D12MA.VirtualAllocationDescription()
		{
			Size = (ulong)count,
			Alignment = 0,
			Flags = flags
		}, out var allocation, out var offset).Success, "Failed to allocate space from buffer");

		// Create a handle from the D3D12MA allocation.
		var alloc = new BufferAllocation<T>(this, allocation, (nint)offset, count);

		allocations.Add(alloc);
		statsDirty = true;

		return alloc;
	}

	public void Free(BufferAllocation<T> alloc)
	{
		if (!alloc.TryMarkFreed())
		{
			return;
		}

		virtualBlock.FreeAllocation(alloc.Handle);

		allocations.Remove(alloc);
		statsDirty = true;
	}

	private void RefreshStats()
	{
		if (!statsDirty)
		{
			return;
		}

		firstOffset = 0;
		lastOffset = 0;

		for (int i = 0; i < allocations.Count; i++)
		{
			nint offset = allocations[i].Offset;

			if (i == 0 || offset < firstOffset)
			{
				firstOffset = offset;
			}
			if (offset > lastOffset)
			{
				lastOffset = offset;
			}
		}

		statsDirty = false;
	}

	public void Clear()
	{
	    virtualBlock.Clear();
	}
}

public class BufferAllocation<T> : IDisposable where T : unmanaged
{
	public nint Offset { get; } = 0;
	public nint Size { get; } = 0;
	public nint End => Offset + Size;
	
	internal D3D12MA.VirtualAllocation Handle;
	public TypedBuffer<T> Buffer { get; private set; }

	private bool isFreed = false;

	internal bool TryMarkFreed()
	{
		if (isFreed)
		{
			return false;
		}

		isFreed = true;
		return true;
	}

	public BufferAllocation(TypedBuffer<T> source, D3D12MA.VirtualAllocation alloc, nint offset, nint size)
	{
		Buffer = source;
		Handle = alloc;
		Offset = offset;
		Size = size;
	}

	public void Dispose()
	{
		Buffer.Free(this);
	}
}