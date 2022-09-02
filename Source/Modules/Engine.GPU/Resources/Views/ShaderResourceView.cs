using System;
using Vortice.Direct3D12;
using Vortice.DXGI;

namespace Engine.GPU
{
	public class ShaderResourceView
	{
		internal static DescriptorHeap Heap = new DescriptorHeap(HeapType.SRV, 4096, true);
		public DescriptorHandle Handle;

		public ShaderResourceView(ID3D12Resource resource, int stride, int capacity, bool isRaw)
		{
			Handle = Heap.Allocate();

			ShaderResourceViewDescription desc = new()
			{
				Format = isRaw ? Format.R32_Typeless : Format.Unknown,
				ViewDimension = ShaderResourceViewDimension.Buffer,
				Shader4ComponentMapping = ShaderComponentMapping.Default,
				Buffer = new()
				{
					FirstElement = 0,
					StructureByteStride = isRaw ? 0 : stride,
					NumElements = isRaw ? capacity / 4 : capacity,
					Flags = isRaw ? BufferShaderResourceViewFlags.Raw : BufferShaderResourceViewFlags.None,
				}
			};

			GPUContext.Device.CreateShaderResourceView(resource, desc, Handle);
		}

		public ShaderResourceView(Texture target)
		{
			Handle = Heap.Allocate();

			ShaderResourceViewDescription desc = new()
			{
				ViewDimension = ShaderResourceViewDimension.Texture2D,
				Shader4ComponentMapping = ShaderComponentMapping.Default,
				Format = target.SRFormat == default ? target.Format : target.SRFormat,
				Texture2D = new()
				{
					MipLevels = 1,
					MostDetailedMip = 0,
					PlaneSlice = 0,
					ResourceMinLODClamp = 0,
				},
			};

			GPUContext.Device.CreateShaderResourceView(target.Resource, desc, Handle);
		}
	}
}