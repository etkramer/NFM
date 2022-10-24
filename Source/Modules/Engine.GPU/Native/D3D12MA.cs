using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using SharpGen.Runtime;
using Vortice.Direct3D12;

namespace Engine.GPU.Native
{
	public static partial class D3D12MA
	{
		[LibraryImport("D3D12MA")]
		private static partial int CreateVirtualBlock(ref VirtualBlockDescription pDesc, out IntPtr ppVirtualBlock);
		[LibraryImport("D3D12MA")]
		private static partial ulong VirtualBlock_Release(IntPtr block);
		[LibraryImport("D3D12MA")]
		private static partial void VirtualBlock_Clear(IntPtr block);
		[LibraryImport("D3D12MA")]
		private static partial int VirtualBlock_Allocate(IntPtr block, IntPtr pDesc, IntPtr pAllocation, IntPtr pOffset);
		[LibraryImport("D3D12MA")]
		private static partial int VirtualBlock_FreeAllocation(IntPtr block, IntPtr pAllocation);
		[LibraryImport("D3D12MA")]
		private static partial int VirtualBlock_IsEmpty(IntPtr block);
		[LibraryImport("D3D12MA")]
		private static partial void VirtualBlock_GetAllocationInfo(IntPtr block, IntPtr pAllocation, IntPtr pAllocationInfo);

		public static Result CreateVirtualBlock(VirtualBlockDescription blockDescription, out VirtualBlock block)
		{
			IntPtr blockPtr = default;
			var result = CreateVirtualBlock(ref blockDescription, out blockPtr);

			block = new VirtualBlock(blockPtr);
			return result;
		}

		public class VirtualBlock : IDisposable
		{
			private IntPtr handle;

			public VirtualBlock(IntPtr nativePtr)
			{
				handle = nativePtr;
			}

			public Result Allocate(VirtualAllocationDescription description, out VirtualAllocation allocation, out ulong offset)
			{
				unsafe
				{
					fixed (VirtualAllocation* allocationPtr = &allocation)
					fixed (ulong* offsetPtr = &offset)
					{
						return VirtualBlock_Allocate(handle, (nint)(&description), (nint)allocationPtr, (nint)offsetPtr);
					}
				}
			}

			public void FreeAllocation(VirtualAllocation allocation)
			{
				unsafe
				{
					VirtualBlock_FreeAllocation(handle, (nint)(&allocation));
				}
			}

			public void GetAllocationInfo(VirtualAllocation allocation, out VirtualAllocationInfo info)
			{
				unsafe
				{
					fixed (VirtualAllocationInfo* infoPtr = &info)
					{
						VirtualBlock_GetAllocationInfo(handle, (nint)(&allocation), (nint)infoPtr);
					}
				}
			}

			public bool IsEmpty() => VirtualBlock_IsEmpty(handle) != 0;
			public void Clear() => VirtualBlock_Clear(handle);
			public ulong Release() => VirtualBlock_Release(handle);
			public void Dispose() => Release();
		}

		public struct VirtualAllocation
		{
			/// <summary>
			/// Unique idenitfier of current allocation. 0 means null/invalid.
			/// </summary>
			public ulong AllocHandle;
		}

		public unsafe struct VirtualAllocationInfo
		{
			/// <summary>
			/// Offset of the allocation.
			/// </summary>
			public ulong Offset;

			/// <summary>
			/// Size of the allocation.
			/// Same value as passed in VIRTUAL_ALLOCATION_DESC::Size.
			/// </summary>
			public ulong Size;

			/// <summary>
			/// Custom pointer associated with the allocation.
			/// Same value as passed in VIRTUAL_ALLOCATION_DESC::pPrivateData or VirtualBlock::SetAllocationPrivateData().
			/// </summary>
			public void* PrivateData;
		}

		public unsafe struct VirtualAllocationDescription
		{
			public VirtualAllocationDescription(ulong size, ulong alignment = 0, VirtualAllocationFlags flags = VirtualAllocationFlags.None)
			{
				Size = size;
				Alignment = alignment;
				Flags = flags;
				PrivateData = null;
			}

			/// <summary>
			/// Flags.
			/// </summary>
			public VirtualAllocationFlags Flags;

			/// <summary>
			/// Size of the allocation.
			/// </summary>
			public ulong Size;

			/// <summary>
			/// Required alignment of the allocation.
			/// </summary>
			public ulong Alignment;

			/// <summary>
			/// Custom pointer to be associated with the allocation.
			/// </summary>
			public void* PrivateData;
		}

		public enum VirtualAllocationFlags
		{
			/// <summary>
			/// Zero.
			/// </summary>
			None = 0,

			/// <summary>
			/// Allocation will be created from upper stack in a double stack pool.
			/// This flag is only allowed for virtual blocks created with VIRTUAL_BLOCK_FLAG_ALGORITHM_LINEAR flag.
			/// </summary>
			UpperAddress = 8,

			/// <summary>
			/// Allocation strategy that tries to minimize memory usage.
			/// </summary>
			MinMemory = 65536,

			/// <summary>
			/// Allocation strategy that tries to minimize allocation time.
			/// </summary>
			MinTime = 131072,

			/// <summary>
			/// Allocation strategy that chooses always the lowest offset in available space. This is not the most efficient strategy but achieves highly packed data.
			/// </summary>
			MinOffset = 16384,
		}

		public unsafe struct VirtualBlockDescription
		{
			/// <summary>
			/// Flags.
			/// </summary>
			public VirtualBlockFlags Flags;

			/// <summary>
			/// Total size of the block.
			/// Sizes can be expressed in bytes or any units you want as long as you are consistent in using them. For example, if you allocate from some array of structures, 1 can mean single instance of entire structure.
			/// </summary>
			public ulong Size;

			/// <summary>
			/// Custom CPU memory allocation callbacks. Optional.
			/// Optional, can be null. When specified, will be used for all CPU-side memory allocations.
			/// </summary>
			public void* AllocationCallbacks;
		}

		public enum VirtualBlockFlags
		{
			/// <summary>
			/// Zero.
			/// </summary>
			None = 0,

			/// <summary>
			/// Enables alternative, linear allocation algorithm in this virtual block.
			/// Specify this flag to enable linear allocation algorithm, which always creates new allocations after last one and doesn't reuse space from allocations freed in between. It trades memory consumption for simplified algorithm and data structure, which has better performance and uses less memory for metadata.
			/// By using this flag, you can achieve behavior of free-at-once, stack, ring buffer, and double stack. For details, see documentation chapter Linear allocation algorithm.
			/// </summary>
			AlgorithmLinear = 1,
		}
	}
}
