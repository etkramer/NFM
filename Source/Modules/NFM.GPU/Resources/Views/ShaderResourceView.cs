using System;
using Vortice.Direct3D12;
using Vortice.DXGI;

namespace NFM.GPU;

public class ShaderResourceView : IDisposable
{
	internal static DescriptorHeap Heap = new DescriptorHeap(HeapType.SRV, 4096, true);
	internal DescriptorHandle Handle;

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

		Guard.NotNull(D3DContext.Device).CreateShaderResourceView(resource, desc, Handle);
	}

	public ShaderResourceView(Texture target, int mipLevel = -1)
	{
		Handle = Heap.Allocate();

		ShaderResourceViewDescription desc = new()
		{
			ViewDimension = ShaderResourceViewDimension.Texture2D,
			Shader4ComponentMapping = ShaderComponentMapping.Default,
			Format = target.SRFormat == default ? target.Format : target.SRFormat,
			Texture2D = new()
			{
				MipLevels = mipLevel == -1 ? target.MipmapCount : 1,
				MostDetailedMip = mipLevel == -1 ? 0 : mipLevel,
				PlaneSlice = 0,
				ResourceMinLODClamp = 0,
			},
		};

		Guard.NotNull(D3DContext.Device).CreateShaderResourceView(target.D3DResource, desc, Handle);
	}

	public void Dispose()
	{
		Handle.Dispose();
	}
}