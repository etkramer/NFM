using System;
using Vortice.DXGI;
using Vortice.Direct3D12;
using System.Runtime.InteropServices;
using Engine.Aspects;
using System.Runtime.CompilerServices;

namespace Engine.GPU
{
	internal unsafe static class UploadHelper
	{
		// 250MB upload heap. TODO: Use a proper allocator, current method means we can't upload to an offset of >250MB.
		const int UploadSize = 250 * 1024 * 1024;

		public static int UploadOffset = 0;
		public static int Ring => GPUContext.FrameIndex;
		public static ID3D12Resource[] Rings;
		public static void*[] MappedRings;

		public static object Lock = new();

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
			Rings = new ID3D12Resource[GPUContext.RenderLatency];
			MappedRings = new void*[GPUContext.RenderLatency];
			for (int i = 0; i < GPUContext.RenderLatency; i++)
			{
				GPUContext.Device.CreateCommittedResource(HeapProperties.UploadHeapProperties, HeapFlags.None, copyBufferDescription, ResourceStates.GenericRead, out Rings[i]);

				void* mapPtr = null;
				Rings[i].Map(0, &mapPtr);
				MappedRings[i] = mapPtr;
			}

			// Reset the upload offset at the beginning of every frame.
			Graphics.OnFrameStart += () =>
			{
				lock (Lock)
				{
					UploadOffset = 0;
				}
			};
		}
	}
}