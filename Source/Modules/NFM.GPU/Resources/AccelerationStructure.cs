using Vortice.Direct3D12;
using Vortice.DXGI;

namespace NFM.GPU;

/// <summary>
/// A ray tracing acceleration structure. Created in - and never leaving - its own resource state,
/// so it's exempt from the usual transition tracking.
/// </summary>
public class AccelerationStructure : RawBuffer
{
	private ShaderResourceView? asView;

	public AccelerationStructure(ulong sizeBytes) : base((nint)sizeBytes, 1, initialState: ResourceStates.RaytracingAccelerationStructure)
	{
		Name = nameof(AccelerationStructure);
	}

	// An acceleration structure is addressed directly, so its view ignores the underlying resource.
	public override ShaderResourceView GetSRV() => asView ??= new ShaderResourceView(GPUAddress);

	public override void Dispose()
	{
		asView?.Dispose();
		base.Dispose();
	}
}

/// <summary>
/// An acceleration structure over one range of a shared vertex/index buffer. Geometry that deforms
/// is built once and refit from then on.
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

		// Grow generously, so a scene that adds instances steadily doesn't reallocate every frame.
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
/// Bump allocator over a single scratch buffer. Every build in a frame gets a disjoint range, so
/// builds overlap freely; running out wraps back to the start behind a barrier.
/// </summary>
public class ScratchAllocator : IDisposable
{
	private const ulong Alignment = D3D12.RaytracingAccelerationStructureByteAlignment;

	private readonly RawBuffer buffer;
	private ulong offset = 0;

	public ScratchAllocator(nint sizeBytes)
	{
		// Left in the common state, which buffers promote out of on first use.
		buffer = new RawBuffer(sizeBytes, 1) { Name = "AS Scratch" };
	}

	public void Reset(CommandList list)
	{
		list.BarrierUAV(buffer);
		offset = 0;
	}

	public ulong Allocate(CommandList list, ulong size)
	{
		size = (size + Alignment - 1) & ~(Alignment - 1);
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
