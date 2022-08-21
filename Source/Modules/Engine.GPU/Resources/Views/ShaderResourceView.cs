using System;
using Vortice.Direct3D12;
using Vortice.DXGI;

namespace Engine.GPU
{
	internal class ShaderResourceView
	{
		internal static DescriptorHeap Heap = new DescriptorHeap(HeapType.SRV, 4096, true);
		internal DescriptorHandle Handle;

		public ShaderResourceView(ID3D12Resource resource, int stride, int capacity)
		{
			Handle = Heap.Allocate();

			ShaderResourceViewDescription desc = new()
			{
				ViewDimension = ShaderResourceViewDimension.Buffer,
				Shader4ComponentMapping = ShaderComponentMapping.Default,
				Buffer = new()
				{
					FirstElement = 0,
					StructureByteStride = stride,
					NumElements = capacity / stride,
					Flags = BufferShaderResourceViewFlags.None,
				}
			};

			GPUContext.Device.CreateShaderResourceView(resource, desc, Handle);
		}

		public ShaderResourceView(Texture texture)
		{
			Handle = Heap.Allocate();

			ShaderResourceViewDescription desc = new()
			{
				ViewDimension = ShaderResourceViewDimension.Texture2D,
				Shader4ComponentMapping = ShaderComponentMapping.Default,
				Texture2D = new()
				{
					MipLevels = 1,
					MostDetailedMip = 0,
					PlaneSlice = 0,
					ResourceMinLODClamp = 0,
				},
			};

			GPUContext.Device.CreateShaderResourceView(texture.Resource, desc, Handle);
		}
	}
}