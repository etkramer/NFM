using System;
using Vortice.DXGI;
using Vortice.Direct3D12;
using System.Runtime.InteropServices;
using Engine.Aspects;
using System.Runtime.CompilerServices;

namespace Engine.GPU
{
	internal unsafe static class UploadBuffer
	{
		// 250MB upload heap. TODO: Use a proper allocator, current method means we can't upload to an offset of >250MB.
		const int UploadSize = 250 * 1024 * 1024;

		public static int UploadOffset = 0;
		public static int Ring => GPUContext.FrameIndex;
		public static ID3D12Resource[] Rings;
		public static void*[] MappedRings;

		public static object Lock = new();

		static UploadBuffer()
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

	[AutoDispose]
	public unsafe partial class GraphicsBuffer : Resource, IDisposable
	{
		public const int ConstantAlignment = 256;
		public int Capacity;
		public int Stride;
		public int Alignment = 1;

		internal ID3D12Resource Resource;

		private ShaderResourceView srv;
		internal ShaderResourceView SRV
		{
			get
			{
				if (srv == null)
				{
					srv = new ShaderResourceView(Resource, Stride, Capacity);
				}

				return srv;
			}
		}

		private UnorderedAccessView uav;
		internal UnorderedAccessView UAV
		{
			get
			{
				if (uav == null)
				{
					uav = new UnorderedAccessView(Resource, Stride, Capacity);
				}

				return uav;
			}
		}

		private ConstantBufferView cbv;
		internal ConstantBufferView CBV
		{
			get
			{
				if (cbv == null)
				{
					Debug.Assert(Capacity * Stride < 65536, "Buffers larger than 64kb cannot be used as program constants");
					Debug.Assert((Capacity * Stride % 256 == 0) || (Alignment % 256 == 0), "Buffers must be aligned to 256b to be used as program constants");
					cbv = new ConstantBufferView(Resource, Stride, Capacity);
				}

				return cbv;
			}
		}

		public string Name
		{
			get => Resource.Name;
			set => Resource.Name = value;
		}

		public GraphicsBuffer(int sizeBytes, int stride, int alignment = 1)
		{
			Capacity = sizeBytes / stride;
			Alignment = alignment;
			Stride = stride;

			ulong width = (ulong)sizeBytes;
			width = MathHelper.Align(width, alignment); // Use user-defined alignment

			// Describe buffer.
			ResourceDescription bufferDescription = new()
			{
				Dimension = ResourceDimension.Buffer,
				Width = width,
				Height = 1,
				DepthOrArraySize = 1,
				MipLevels = 1,
				Format = Format.Unknown,
				SampleDescription = new(1, 0),
				Layout = TextureLayout.RowMajor,
				Flags = ResourceFlags.AllowUnorderedAccess,
			};

			// Create buffer.
			GPUContext.Device.CreateCommittedResource(HeapProperties.DefaultHeapProperties, HeapFlags.None, bufferDescription, ResourceStates.CopyDest, out Resource);
			State = ResourceStates.CopyDest;

			// Set debug name.
			Name = GetType().Name;
		}

		public void Dispose()
		{
			Resource.Dispose();
		}

		internal override ID3D12Resource GetBaseResource()
		{
			return Resource;
		}

		public void SetData(void* data, int dataSize, long offset)
		{
			lock (UploadBuffer.Lock)
			{
				int uploadOffset = UploadBuffer.UploadOffset;
				long destOffset = offset;

				// Copy data to upload buffer.
				Unsafe.CopyBlockUnaligned((byte*)UploadBuffer.MappedRings[UploadBuffer.Ring] + uploadOffset, data, (uint)dataSize);
				UploadBuffer.UploadOffset += dataSize;

				// Copy from upload to target buffer.
				Graphics.CustomCommand((o) => {
					o.CopyBufferRegion(Resource, (ulong)destOffset, UploadBuffer.Rings[UploadBuffer.Ring], (ulong)uploadOffset, (ulong)dataSize);
				}, new[] {
					new CommandInput() {
						Resource = this,
						State = ResourceStates.CopyDest
					}
				});
			}
		}
	}
}