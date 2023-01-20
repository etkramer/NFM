using System;
using NFM.GPU.Native;
using Vortice.Direct3D12;

namespace NFM.GPU;

public enum HeapType
{
	CBV = DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView,
	SRV = DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView,
	UAV = DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView,
	DSV = DescriptorHeapType.DepthStencilView,
	RTV = DescriptorHeapType.RenderTargetView,
	Sampler = DescriptorHeapType.Sampler,
}

public struct DescriptorHandle : IDisposable
{
	public DescriptorHeap Heap;
	public int Index;
	internal D3D12MA.VirtualAllocation Allocation;

	public static implicit operator CpuDescriptorHandle(DescriptorHandle handle) => handle.CPUHandle;
	public static implicit operator GpuDescriptorHandle(DescriptorHandle handle) => handle.GPUHandle;

	public CpuDescriptorHandle CPUHandle => Heap.GetCPUHandle(Index);
	public GpuDescriptorHandle GPUHandle => Heap.GetGPUHandle(Index);

	public void Dispose()
	{
		Heap.Free(this);
	}
}

public class DescriptorHeap : IDisposable
{
	internal ID3D12DescriptorHeap handle;
	private D3D12MA.VirtualBlock virtualBlock;
	private int stride;

	public HeapType Type { get; }

	public DescriptorHeap(HeapType type, int capacity, bool shaderVisible)
	{
		Type = type;

		DescriptorHeapDescription heapDesc = new()
		{
			DescriptorCount = capacity + 1,
			Type = (DescriptorHeapType)type,
			Flags = shaderVisible ? DescriptorHeapFlags.ShaderVisible : DescriptorHeapFlags.None,
			NodeMask = 0,
		};

		D3DContext.Device.CreateDescriptorHeap(heapDesc, out handle);
		handle.Name = $"DescriptorHeap ({type})";
		stride = D3DContext.Device.GetDescriptorHandleIncrementSize((DescriptorHeapType)type);

		// Create block with D3D12MA
		D3D12MA.CreateVirtualBlock(new D3D12MA.VirtualBlockDescription()
		{
			Size = (ulong)capacity + 1,
			Flags = D3D12MA.VirtualBlockFlags.None,
		}, out virtualBlock);

		// Reserve first element (0) to represent an invalid index.
		virtualBlock.Allocate(new D3D12MA.VirtualAllocationDescription()
		{
			Size = 1,
			Alignment = 0,
			Flags = D3D12MA.VirtualAllocationFlags.MinOffset
		}, out _, out _);
	}

	public DescriptorHandle Allocate()
	{
		virtualBlock.Allocate(new D3D12MA.VirtualAllocationDescription()
		{
			Size = 1,
			Alignment = 0,
			Flags = D3D12MA.VirtualAllocationFlags.None
		}, out var allocation, out _);

		virtualBlock.GetAllocationInfo(allocation, out var info);

		return new DescriptorHandle()
		{
			Heap = this,
			Index = (int)info.Offset,
			Allocation = allocation,
		};
	}

	public void Free(DescriptorHandle handle)
	{
		virtualBlock.FreeAllocation(handle.Allocation);
	}

	public IntPtr GetPointer()
	{
		return handle.NativePointer;
	}

	public CpuDescriptorHandle GetCPUHandle(int index)
	{
		return handle.GetCPUDescriptorHandleForHeapStart() + (index * stride);
	}

	public GpuDescriptorHandle GetGPUHandle(int index)
	{
		return handle.GetGPUDescriptorHandleForHeapStart() + (index * stride);
	}

	public static implicit operator ID3D12DescriptorHeap(DescriptorHeap heap)
	{
		return heap.handle;
	}

	public void Dispose()
	{
		handle.Dispose();
	}
}
