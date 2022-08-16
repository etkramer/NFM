﻿using System;
using Vortice.Direct3D12;
using Vortice.DXGI;

namespace Engine.GPU
{
	internal class ConstantBufferView
	{
		public DescriptorHandle Handle;

		public ConstantBufferView(ID3D12Resource resource, int stride, int capacity)
		{
			Handle = ShaderResourceView.Heap.Allocate();

			ConstantBufferViewDescription desc = new()
			{
				BufferLocation = resource.GPUVirtualAddress,
				SizeInBytes = stride * capacity
			};

			GPUContext.Device.CreateConstantBufferView(desc, Handle);
		}
	}
}