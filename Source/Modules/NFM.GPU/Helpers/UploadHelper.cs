using System;
using Vortice.DXGI;
using Vortice.Direct3D12;

namespace NFM.GPU
{
	internal unsafe static class UploadHelper
	{
		// 512MB upload heap. TODO: Use a proper allocator, current method means we can't upload to an offset of >512MB.
		const int UploadSize = 512 * 1024 * 1024;

		public static nint UploadOffset = 0;
		public static int Ring => D3DContext.FrameIndex;
		public static ID3D12Resource[] Rings;
		public static void*[] MappedRings;

		public static object Lock { get; } = new();

		static UploadHelper()
		{
			ResourceDescription copyBufferDescription = new()
			{
				Dimension = ResourceDimension.Buffer,
				Alignment = 0,
				Width = UploadSize,
				Height = 1,
				DepthOrArraySize = 1,
				MipLevels = 1,
				Format = Format.Unknown,
				SampleDescription = new(1, 0),
				Layout = TextureLayout.RowMajor,
				Flags = ResourceFlags.None,
			};

			// Create upload rings.
			Rings = new ID3D12Resource[D3DContext.RenderLatency];
			MappedRings = new void*[D3DContext.RenderLatency];
			for (int i = 0; i < D3DContext.RenderLatency; i++)
			{
				D3DContext.Device.CreateCommittedResource(HeapProperties.UploadHeapProperties, HeapFlags.None, copyBufferDescription, ResourceStates.GenericRead, out Rings[i]);

				void* mapPtr = null;
				Rings[i].Map(0, &mapPtr);
				MappedRings[i] = mapPtr;
			}

			// Reset the upload offset at the beginning of every frame.
			D3DContext.OnFrameStart += () =>
			{
				lock (Lock)
				{
					UploadOffset = 0;
				}
			};
		}
	}
}