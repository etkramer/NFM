using System;
using Vortice.Direct3D12;

namespace Engine.GPU
{
	public enum HeapType
	{
		CBV = DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView,
		SRV = DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView,
		UAV = DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView,
		DSV = DescriptorHeapType.DepthStencilView,
		RTV = DescriptorHeapType.RenderTargetView,
		Sampler = DescriptorHeapType.Sampler,
	}

	public struct DescriptorHandle
	{
		public DescriptorHeap Heap;
		public int Index;

		public static implicit operator CpuDescriptorHandle(DescriptorHandle handle) => handle.CPUHandle;
		public static implicit operator GpuDescriptorHandle(DescriptorHandle handle) => handle.GPUHandle;

		public CpuDescriptorHandle CPUHandle => Heap.GetCPUHandle(Index);
		public GpuDescriptorHandle GPUHandle => Heap.GetGPUHandle(Index);
	}

	public class DescriptorHeap : IDisposable
	{
		internal ID3D12DescriptorHeap handle;
		private int stride;
		private int count;

		public HeapType Type { get; }

		public DescriptorHeap(HeapType type, int descriptorCount, bool shaderVisible)
		{
			Type = type;

			DescriptorHeapDescription heapDesc = new()
			{
				DescriptorCount = descriptorCount,
				Type = (DescriptorHeapType)type,
				Flags = shaderVisible ? DescriptorHeapFlags.ShaderVisible : DescriptorHeapFlags.None,
				NodeMask = 0,
			};

			GPUContext.Device.CreateDescriptorHeap(heapDesc, out handle);
			handle.Name = $"DescriptorHeap ({type})";
			stride = GPUContext.Device.GetDescriptorHandleIncrementSize((DescriptorHeapType)type);
		}

		public DescriptorHandle Allocate()
		{
			return new DescriptorHandle()
			{
				Heap = this,
				Index = count++,
			};
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
}
