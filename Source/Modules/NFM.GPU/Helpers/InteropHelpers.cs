using System;
using Vortice.Direct3D12;

namespace NFM.GPU;

public static class InteropHelpers
{
	public static IntPtr PtrFromList(ID3D12GraphicsCommandList list)
	{
		return list.NativePointer;
	}
}
