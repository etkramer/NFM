using Vortice.Direct3D12;
using Vortice.DXGI;

namespace NFM.GPU;

public unsafe partial class RawBuffer : Resource, IDisposable
{
	public const int ConstantAlignment = D3D12.ConstantBufferDataPlacementAlignment;
	public const int CounterAlignment = D3D12.UnorderedAccessViewCounterPlacementAlignment;
	public nint SizeBytes => (nint)Capacity * Stride;
	public int Capacity;
	public int Stride;
	public int SizeAlignment = 1;

	public bool HasCounter { get; private set; }
	public nint CounterOffset { get; private set; } = 0;
	public bool IsRaw { get; private set; }

	internal override ID3D12Resource D3DResource { get; private protected set; }

	private ShaderResourceView srv;
	private UnorderedAccessView uav;
	private ConstantBufferView cbv;

	public ShaderResourceView GetSRV()
	{
		if (srv == null)
		{
			srv = new ShaderResourceView(D3DResource, Stride, Capacity, IsRaw && Stride == 1);
		}

		return srv;
	}

	public UnorderedAccessView GetUAV()
	{
		if (uav == null)
		{
			uav = new UnorderedAccessView(D3DResource, Stride, Capacity, HasCounter, CounterOffset);
		}

		return uav;
	}

	public ConstantBufferView GetCBV()
	{
		if (cbv == null)
		{
			Debug.Assert((Capacity * Stride % ConstantAlignment == 0) || (SizeAlignment % ConstantAlignment == 0), "Buffers must be aligned to 256b to be used as program constants");
			cbv = new ConstantBufferView(D3DResource, Stride, Capacity);
		}

		return cbv;
	}

	public string Name
	{
		get => D3DResource.Name;
		set => D3DResource.Name = value;
	}

	public RawBuffer(nint sizeBytes, int stride, int sizeAlignment = 1, bool hasCounter = false, bool isRaw = false)
	{
		Capacity = (int)(sizeBytes / stride);
		SizeAlignment = sizeAlignment;
		Stride = stride;
		HasCounter = hasCounter;
		IsRaw = isRaw;

		nint width = sizeBytes;

		// Ensure enough space for UAV counter.
		if (hasCounter)
		{
			width = MathHelper.Align(width, CounterAlignment) + 4;
			CounterOffset = (width - 4);
		}

		// Ensure user-defined alignment.
		width = MathHelper.Align(width, sizeAlignment);

		// Describe buffer.
		ResourceDescription bufferDescription = new()
		{
			Dimension = ResourceDimension.Buffer,
			Width = (ulong)width,
			Height = 1,
			DepthOrArraySize = 1,
			MipLevels = 1,
			Format = Format.Unknown,
			SampleDescription = new(1, 0),
			Layout = TextureLayout.RowMajor,
			Flags = ResourceFlags.AllowUnorderedAccess,
		};

		// Create buffer.
		D3DContext.Device.CreateCommittedResource(HeapProperties.DefaultHeapProperties, HeapFlags.None, bufferDescription, ResourceStates.Common, out ID3D12Resource resource);
		D3DResource = resource;
		State = ResourceStates.Common;

		// Set debug name.
		Name = GetType().Name;
	}

	public void Resize(nint sizeBytes)
	{
		throw new NotImplementedException("Buffer resizing is not yet supported.");
	}

	public virtual void Dispose()
	{
		D3DResource.SafeRelease();
		srv?.Dispose();
		uav?.Dispose();
		cbv?.Dispose();

		IsAlive = false;
	}
}