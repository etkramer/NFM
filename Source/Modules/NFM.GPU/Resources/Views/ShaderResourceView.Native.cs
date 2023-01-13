using System;
using Vortice.Direct3D12;
using Vortice.DXGI;

namespace NFM.GPU.Native
{
	public static class ShaderResourceViewNative
	{
		public static int GetDescriptorIndex(this ShaderResourceView view)
		{
			return view.Handle.Index;
		}
	}
}