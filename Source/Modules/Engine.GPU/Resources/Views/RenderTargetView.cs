using System;
using Vortice.Direct3D12;
using Vortice.DXGI;

namespace Engine.GPU
{
	public class RenderTargetView : IDisposable
	{
		public static DescriptorHeap Heap = new DescriptorHeap(HeapType.RTV, 4096, false);

		public Texture Target;
		public DescriptorHandle Handle;

		public RenderTargetView(Texture target)
		{
			Target = target;
			Handle = Heap.Allocate();

			RenderTargetViewDescription desc = new()
			{
				Format = Target.Format,
				ViewDimension = Target.Samples == 1 ? RenderTargetViewDimension.Texture2D : RenderTargetViewDimension.Texture2DMultisampled,
			};

			GPUContext.Device.CreateRenderTargetView(Target.GetBaseResource(), desc, Handle);
		}

		public void Dispose()
		{

		}
	}
}