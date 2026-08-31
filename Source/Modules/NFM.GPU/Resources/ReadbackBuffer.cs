using Vortice.Direct3D12;
using Vortice.DXGI;

namespace NFM.GPU;

/// <summary>
/// A CPU-visible buffer, used to read GPU results back to the host.
/// Lives permanently in the copy-dest state, so it never needs a barrier.
/// </summary>
public unsafe class ReadbackBuffer : Resource, IDisposable
{
	public nint SizeBytes { get; }

	internal override ID3D12Resource D3DResource { get; private protected set; }

	private void* mappedData;

	public string Name
	{
		get => D3DResource.Name;
		set => D3DResource.Name = value;
	}

	public ReadbackBuffer(nint sizeBytes)
	{
		SizeBytes = sizeBytes;

		ResourceDescription bufferDescription = new()
		{
			Dimension = ResourceDimension.Buffer,
			Alignment = 0,
			Width = (ulong)sizeBytes,
			Height = 1,
			DepthOrArraySize = 1,
			MipLevels = 1,
			Format = Format.Unknown,
			SampleDescription = new(1, 0),
			Layout = TextureLayout.RowMajor,
			Flags = ResourceFlags.None,
		};

		Guard.NotNull(D3DContext.Device).CreateCommittedResource(HeapProperties.ReadbackHeapProperties, HeapFlags.None, bufferDescription, ResourceStates.CopyDest, out var resource);
		D3DResource = Guard.NotNull(resource);
		State = ResourceStates.CopyDest;

		void* mapPtr = null;
		D3DResource.Map(0, &mapPtr);
		mappedData = mapPtr;

		Name = GetType().Name;
	}

	/// <summary>
	/// Reads the contents of the buffer. Only meaningful once the copy that filled it has retired on the GPU.
	/// </summary>
	public ReadOnlySpan<T> Read<T>(int count, nint offset = 0) where T : unmanaged
	{
		Guard.Require(offset + (count * sizeof(T)) <= SizeBytes, "Tried to read past the end of a readback buffer");
		return new ReadOnlySpan<T>((byte*)mappedData + offset, count);
	}

	public void Dispose()
	{
		D3DResource.Unmap(0);
		D3DResource.SafeRelease();

		mappedData = null;
		IsAlive = false;
	}
}
