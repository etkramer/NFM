using System;
using Vortice.Direct3D12;
using Vortice.DXGI;

namespace NFM.GPU
{
	public class DepthStencilView : IDisposable
	{
		public static DescriptorHeap Heap = new DescriptorHeap(HeapType.DSV, 4096, false);

		public Texture Target;
		public DescriptorHandle Handle;

		public DepthStencilView(Texture target)
		{
			Target = target;
			Handle = Heap.Allocate();

			DepthStencilViewDescription desc = new()
			{
				Format = Target.DSFormat == default ? Target.Format : Target.DSFormat,
				ViewDimension = Target.Samples == 1 ? DepthStencilViewDimension.Texture2D : DepthStencilViewDimension.Texture2DMultisampled,
				Flags = DepthStencilViewFlags.None
			};

			Graphics.Device.CreateDepthStencilView(Target.D3DResource, desc, Handle);
		}

		public void Dispose()
		{

		}
	}
}