using System;
using Vortice.DXGI;
using Vortice.Direct3D12;
using System.Runtime.InteropServices;
using Engine.Aspects;
using System.Runtime.CompilerServices;

namespace Engine.GPU
{
	[AutoDispose]
	public unsafe partial class GraphicsBuffer : Resource, IDisposable
	{
		public const int ConstantAlignment = 256;
		public int Capacity;
		public int Stride;
		public int Alignment = 1;

		public bool HasCounter { get; private set; }
		public bool IsRaw { get; private set; }
		public long CounterOffset { get; private set; } = 0;

		internal ID3D12Resource Resource;

		private ShaderResourceView srv;
		internal ShaderResourceView SRV
		{
			get
			{
				if (srv == null)
				{
					srv = new ShaderResourceView(Resource, Stride, Capacity, IsRaw && Stride == 1);
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
					uav = new UnorderedAccessView(Resource, Stride, Capacity, HasCounter, CounterOffset);
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

		public GraphicsBuffer(int sizeBytes, int stride, int alignment = 1, bool hasCounter = false, bool isRaw = false)
		{
			Capacity = sizeBytes / stride;
			Alignment = alignment;
			Stride = stride;
			HasCounter = hasCounter;
			IsRaw = isRaw;

			ulong width = (ulong)sizeBytes;

			// Ensure enough space for UAV counter.
			if (hasCounter)
			{
				const int UAV_COUNTER_PLACEMENT_ALIGNMENT = 4096;
				width = MathHelper.Align(width, UAV_COUNTER_PLACEMENT_ALIGNMENT) + 4;

				CounterOffset = (long)(width - 4);
			}

			// Ensure user-defined alignment.
			width = MathHelper.Align(width, alignment);

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
				int uploadRing = UploadBuffer.Ring;
				int uploadOffset = UploadBuffer.UploadOffset;
				long destOffset = offset;

				// Copy data to upload buffer.
				Unsafe.CopyBlockUnaligned((byte*)UploadBuffer.MappedRings[uploadRing] + uploadOffset, data, (uint)dataSize);
				UploadBuffer.UploadOffset += dataSize;

				// Copy from upload to target buffer.
				Graphics.GetCommandList().CustomCommand((o) => {
					o.CopyBufferRegion(Resource, (ulong)destOffset, UploadBuffer.Rings[uploadRing], (ulong)uploadOffset, (ulong)dataSize);
				}, new[] {
					new CommandInput() {
						Resource = this,
						State = ResourceStates.CopyDest
					}
				});
			}
		}

		public void ResetCounter()
		{
			if (HasCounter)
			{
				int zeroInt = 0;
				SetData(&zeroInt, 4, CounterOffset);
			}
		}
	}
}