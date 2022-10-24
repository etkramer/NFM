using System;
using Vortice.Direct3D12;
using Vortice.DXGI;

namespace Engine.GPU
{
	public class ConstantBufferView : IDisposable
	{
		public DescriptorHandle Handle;

		public ConstantBufferView(ID3D12Resource resource, int stride, int capacity)
		{
			Handle = ShaderResourceView.Heap.Allocate();

			ConstantBufferViewDescription desc = new()
			{
				BufferLocation = resource.GPUVirtualAddress,
				SizeInBytes = (int)MathHelper.Align(capacity * stride, GraphicsBuffer.ConstantAlignment)
			};

			Graphics.Device.CreateConstantBufferView(desc, Handle);
		}

		public void Dispose()
		{

		}
	}
}