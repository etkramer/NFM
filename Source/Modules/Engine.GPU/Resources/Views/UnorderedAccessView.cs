using System;
using Vortice.Direct3D12;
using Vortice.DXGI;

namespace Engine.GPU
{
	internal class UnorderedAccessView
	{
		public DescriptorHandle Handle;

		public UnorderedAccessView(ID3D12Resource resource, int stride, int capacity)
		{
			Handle = ShaderResourceView.Heap.Allocate();

			UnorderedAccessViewDescription desc = new()
			{
				Format = Format.Unknown,
				ViewDimension = UnorderedAccessViewDimension.Buffer,
				Buffer = new()
				{
					FirstElement = 0,
					StructureByteStride = stride,
					NumElements = capacity / stride,
					Flags = BufferUnorderedAccessViewFlags.None,
					CounterOffsetInBytes = 0,
				}
			};

			GPUContext.Device.CreateUnorderedAccessView(resource, null, desc, Handle);
		}
	}
}