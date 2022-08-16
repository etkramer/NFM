using System;
using Vortice.Direct3D12;
using Vortice.DXGI;

namespace Engine.GPU
{
	internal class DepthStencilView
	{
		public static DescriptorHeap Heap = new DescriptorHeap(HeapType.DSV, 4096, false);

		public Texture Target;
		public DescriptorHandle Handle;

		public DepthStencilView(Texture target)
		{
			Target = target;
			Handle = Heap.Allocate();

			Recreate();
		}

		public void Recreate()
		{
			DepthStencilViewDescription desc = new()
			{
				Format = Target.Format,
				ViewDimension = DepthStencilViewDimension.Texture2D,
				Flags = DepthStencilViewFlags.None
			};

			GPUContext.Device.CreateDepthStencilView(Target.GetBaseResource(), desc, Handle);
		}
	}
}