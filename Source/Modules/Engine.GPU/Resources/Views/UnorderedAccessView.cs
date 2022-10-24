using System;
using Vortice.Direct3D12;
using Vortice.DXGI;

namespace Engine.GPU
{
	public class UnorderedAccessView : IDisposable
	{
		internal DescriptorHandle Handle;

		public UnorderedAccessView(ID3D12Resource resource, int stride, int capacity, bool hasCounter, long counterOffset)
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
					NumElements = capacity,
					Flags = BufferUnorderedAccessViewFlags.None,
					CounterOffsetInBytes = (ulong)counterOffset,
				}
			};

			Graphics.Device.CreateUnorderedAccessView(resource, hasCounter ? resource : null, desc, Handle);
		}

		public UnorderedAccessView(Texture texture, int mipLevel)
		{
			Handle = ShaderResourceView.Heap.Allocate();

			UnorderedAccessViewDescription desc = new()
			{
				Format = texture.Format,
				ViewDimension = UnorderedAccessViewDimension.Texture2D,
				Texture2D = new()
				{
					MipSlice = mipLevel,
					PlaneSlice = 0
				}
			};

			Graphics.Device.CreateUnorderedAccessView(texture, null, desc, Handle);
		}

		public void Dispose()
		{

		}
	}
}