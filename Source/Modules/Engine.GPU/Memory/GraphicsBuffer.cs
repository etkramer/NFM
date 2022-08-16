using System;
using Vortice.DXGI;
using Vortice.Direct3D12;
using System.Runtime.InteropServices;
using Engine.Aspects;

namespace Engine.GPU
{
	internal unsafe static class UploadBuffer
	{
		// 250MB upload heap. TODO: Use a proper allocator, current method means we can't upload to an offset of >250MB.
		const int UploadSize = 250 * 1024 * 1024;
		internal static ID3D12Resource Resource;

		internal static int UploadOffset = 0;

		static UploadBuffer()
		{
			ResourceDescription copyBufferDescription = new()
			{
				Dimension = ResourceDimension.Buffer,
				Alignment = 0,
				Width = (ulong)UploadSize,
				Height = 1,
				DepthOrArraySize = 1,
				MipLevels = 1,
				Format = Format.Unknown,
				SampleDescription = new(1, 0),
				Layout = TextureLayout.RowMajor,
				Flags = ResourceFlags.None,
			};

			GPUContext.Device.CreateCommittedResource(HeapProperties.UploadHeapProperties, HeapFlags.None, copyBufferDescription, ResourceStates.GenericRead, out Resource);
		}

		public static void ResetFrame()
		{
			UploadOffset = 0;
		}
	}

	public unsafe class GraphicsBuffer : GraphicsBuffer<byte>
	{
		public GraphicsBuffer(int capacityBytes) : base(capacityBytes) {}
	}

	[AutoDispose]
	public unsafe partial class GraphicsBuffer<T> : Resource, IDisposable where T : unmanaged
	{
		public int Capacity;

		internal ID3D12Resource Resource;

		private ShaderResourceView srv;
		internal ShaderResourceView SRV
		{
			get
			{
				if (srv == null)
				{
					srv = new ShaderResourceView(Resource, sizeof(T), Capacity);
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
					uav = new UnorderedAccessView(Resource, sizeof(T), Capacity);
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
					Debug.Assert(Capacity * sizeof(T) < 65536, "Buffers larger than 64kb cannot be used as program constants");
					cbv = new ConstantBufferView(Resource, sizeof(T), Capacity);
				}

				return cbv;
			}
		}

		public string Name
		{
			get => Resource.Name;
			set => Resource.Name = value;
		}

		public GraphicsBuffer(int capacity)
		{
			Capacity = capacity;

			// Describe buffer.
			ResourceDescription bufferDescription = new()
			{
				Dimension = ResourceDimension.Buffer,
				Alignment = 0,
				Width = (ulong)capacity * (ulong)sizeof(T),
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

			// Create initial allocation block.
			blocks.Add(new Block()
			{
				Start = 0,
				Length = Capacity,
				Free = true,
			});
		}

		public void Dispose()
		{
			Resource.Dispose();
		}

		internal override ID3D12Resource GetBaseResource()
		{
			return Resource;
		}

		/// <summary>
		/// Allocates space for the data, then uploads it to the GPU.
		/// </summary>
		public BufferHandle<T> Upload(T[] data)
		{
			var handle = Allocate(data.Length);
			SetData(handle, data);
			return handle;
		}

		/// <inheritdoc cref="Upload(T[])"/>
		public BufferHandle<T> Upload(T data)
		{
			var handle = Allocate(1);
			SetData(handle, data);
			return handle;
		}

		/// <inheritdoc cref="Upload(T[])"/>
		public BufferHandle<T> Upload(Span<T> data)
		{
			var handle = Allocate(data.Length);
			SetData(handle, data);
			return handle;
		}

		public void SetData(BufferHandle<T> handle, T[] data) => SetData(handle, data.AsSpan());
		public void SetData(BufferHandle<T> handle, T data) => SetData(handle, MemoryMarshal.CreateSpan(ref data, sizeof(T)));
		public void SetData(BufferHandle<T> handle, Span<T> data) => SetData(handle.ElementStart, data);

		public void SetData(long start, T[] data) => SetData(start, data.AsSpan());
		public void SetData(long start, T data) => SetData(start, MemoryMarshal.CreateSpan(ref data, 1));
		public void SetData(long start, Span<T> data)
		{
			int dataSize = data.Length * sizeof(T);

			int uploadOffset = UploadBuffer.UploadOffset;
			int destOffset = (int)(start * sizeof(T));

			// Copy data to upload buffer.
			UploadBuffer.Resource.SetData((ReadOnlySpan<T>)data, UploadBuffer.UploadOffset);
			UploadBuffer.UploadOffset += dataSize;

			// Copy from upload to target buffer.
			Graphics.CustomCommand((o) => {
				o.CopyBufferRegion(Resource, (ulong)destOffset, UploadBuffer.Resource, (ulong)uploadOffset, (ulong)dataSize);
			}, new[] {
				new CommandInput() {
					Resource = this,
					State = ResourceStates.CopyDest
				}
			});

			// Schedule a reset of the upload buffer.
			Queue.Schedule(() => UploadBuffer.UploadOffset = 0, -2);
		}

		public T this[int i]
		{
			set
			{
				SetData(i, value);
			}
		}
	}
}