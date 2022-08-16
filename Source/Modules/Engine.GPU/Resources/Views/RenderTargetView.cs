using System;
using Vortice.Direct3D12;
using Vortice.DXGI;

namespace Engine.GPU
{
	internal class RenderTargetView
	{
		public static DescriptorHeap Heap = new DescriptorHeap(HeapType.RTV, 4096, false);

		public Texture Target;
		public DescriptorHandle Handle;

		public RenderTargetView(Texture target)
		{
			Target = target;
			Handle = Heap.Allocate();

			Recreate();
		}

		public void Recreate()
		{
			RenderTargetViewDescription desc = new()
			{
				Format = Target.Format, ViewDimension = RenderTargetViewDimension.Texture2D,
			};

			GPUContext.Device.CreateRenderTargetView(Target.GetBaseResource(), desc, Handle);
		}
	}
}