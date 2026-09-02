using Vortice.Direct3D12;
using Vortice.DXGI;

namespace NFM.GPU;

/// <summary>
/// A ray tracing acceleration structure, pinned to its own resource state for life.
/// </summary>
public class AccelerationStructure : RawBuffer
{
	private ShaderResourceView? asView;

	public AccelerationStructure(ulong sizeBytes) : base((nint)sizeBytes, 1, initialState: ResourceStates.RaytracingAccelerationStructure)
	{
		Name = nameof(AccelerationStructure);
	}

	// Addressed directly, so the view ignores the underlying resource.
	public override ShaderResourceView GetSRV() => asView ??= new ShaderResourceView(GPUAddress);

	public override void Dispose()
	{
		asView?.Dispose();
		base.Dispose();
	}
}

/// <summary>
/// An acceleration structure over one range of a shared vertex/index buffer.
/// </summary>
public class BottomLevelAS : IDisposable
{
	public ulong GPUAddress => structure.GPUAddress;
	public ulong ScratchSize { get; }

	private readonly AccelerationStructure structure;
	private readonly BuildRaytracingAccelerationStructureInputs inputs;
	private readonly bool allowUpdate;

	private bool isBuilt = false;

	public BottomLevelAS(RawBuffer vertices, nint vertexOffset, nint vertexCount, RawBuffer indices, nint indexOffset, nint indexCount, bool allowUpdate = false)
	{
		this.allowUpdate = allowUpdate;

		RaytracingGeometryDescription geometry = new()
		{
			Type = RaytracingGeometryType.Triangles,
			Flags = RaytracingGeometryFlags.Opaque,
			Triangles = new()
			{
				VertexBuffer = new(vertices.GPUAddress + (ulong)(vertexOffset * vertices.Stride), (ulong)vertices.Stride),
				VertexCount = (int)vertexCount,
				VertexFormat = Format.R32G32B32_Float,
				IndexBuffer = indices.GPUAddress + (ulong)(indexOffset * indices.Stride),
				IndexCount = (int)indexCount,
				IndexFormat = Format.R32_UInt,
			}
		};

		inputs = new()
		{
			Type = RaytracingAccelerationStructureType.BottomLevel,
			Layout = ElementsLayout.Array,
			Flags = allowUpdate
				? RaytracingAccelerationStructureBuildFlags.AllowUpdate | RaytracingAccelerationStructureBuildFlags.PreferFastBuild
				: RaytracingAccelerationStructureBuildFlags.PreferFastTrace,
			DescriptorsCount = 1,
			GeometryDescriptions = [geometry],
		};

		var info = Guard.NotNull(D3DContext.Device).GetRaytracingAccelerationStructurePrebuildInfo(inputs);

		structure = new AccelerationStructure(info.ResultDataMaxSizeInBytes) { Name = "BLAS" };
		ScratchSize = Math.Max(info.ScratchDataSizeInBytes, info.UpdateScratchDataSizeInBytes);
	}

	public void Build(CommandList list, ulong scratchAddress)
	{
		bool refit = isBuilt && allowUpdate;
		var buildInputs = inputs;

		if (refit)
		{
			buildInputs.Flags |= RaytracingAccelerationStructureBuildFlags.PerformUpdate;
		}

		list.BuildAccelerationStructure(new()
		{
			Inputs = buildInputs,
			DestinationAccelerationStructureData = structure.GPUAddress,
			ScratchAccelerationStructureData = scratchAddress,
			SourceAccelerationStructureData = refit ? structure.GPUAddress : 0,
		});

		isBuilt = true;
	}

	public void Dispose() => structure.Dispose();
}

/// <summary>
/// The scene's acceleration structure, rebuilt each frame over a GPU-written array of instances.
/// </summary>
public class TopLevelAS : IDisposable
{
	public const int DescStride = 64;

	/// <summary>
	/// Instance descriptions consumed by the next build, filled in by a compute pass.
	/// </summary>
	public RawBuffer Instances { get; private set; } = new(DescStride, DescStride);

	public ulong ScratchSize { get; private set; }

	private AccelerationStructure? structure;
	private int capacity = 0;

	public AccelerationStructure Structure => Guard.NotNull(structure);

	/// <summary>
	/// Grows the structure and its instance array to fit, discarding anything built so far.
	/// </summary>
	public void EnsureCapacity(int instanceCount)
	{
		if (instanceCount <= capacity && structure is not null)
		{
			return;
		}

		capacity = MathHelper.Max(instanceCount * 2, 128);

		var info = Guard.NotNull(D3DContext.Device).GetRaytracingAccelerationStructurePrebuildInfo(new()
		{
			Type = RaytracingAccelerationStructureType.TopLevel,
			Layout = ElementsLayout.Array,
			Flags = RaytracingAccelerationStructureBuildFlags.PreferFastTrace,
			DescriptorsCount = capacity,
		});

		structure?.Dispose();
		Instances.Dispose();

		structure = new AccelerationStructure(info.ResultDataMaxSizeInBytes) { Name = "TLAS" };
		Instances = new RawBuffer(capacity * DescStride, DescStride) { Name = "TLAS Instances" };
		ScratchSize = info.ScratchDataSizeInBytes;
	}

	public void Build(CommandList list, int instanceCount, ulong scratchAddress)
	{
		list.RequestState(Instances, ResourceStates.NonPixelShaderResource);

		list.BuildAccelerationStructure(new()
		{
			Inputs = new()
			{
				Type = RaytracingAccelerationStructureType.TopLevel,
				Layout = ElementsLayout.Array,
				Flags = RaytracingAccelerationStructureBuildFlags.PreferFastTrace,
				DescriptorsCount = instanceCount,
				InstanceDescriptions = Instances.GPUAddress,
			},
			DestinationAccelerationStructureData = Structure.GPUAddress,
			ScratchAccelerationStructureData = scratchAddress,
		});
	}

	public void Dispose()
	{
		structure?.Dispose();
		Instances.Dispose();
	}
}

/// <summary>
/// Bump allocator over a single scratch buffer, wrapping behind a barrier when it runs out.
/// </summary>
public class ScratchAllocator : IDisposable
{
	private const int Alignment = (int)D3D12.RaytracingAccelerationStructureByteAlignment;

	private readonly RawBuffer buffer;
	private ulong offset = 0;

	public ScratchAllocator(nint sizeBytes)
	{
		buffer = new RawBuffer(sizeBytes, 1) { Name = "AS Scratch" };
	}

	public void Reset(CommandList list)
	{
		list.BarrierUAV(buffer);
		offset = 0;
	}

	public ulong Allocate(CommandList list, ulong size)
	{
		size = MathHelper.Align(size, Alignment);
		Guard.Require(size <= (ulong)buffer.SizeBytes, "Acceleration structure needs more scratch than the whole buffer holds.");

		if (offset + size > (ulong)buffer.SizeBytes)
		{
			Reset(list);
		}

		ulong result = buffer.GPUAddress + offset;
		offset += size;

		return result;
	}

	public void Dispose() => buffer.Dispose();
}
