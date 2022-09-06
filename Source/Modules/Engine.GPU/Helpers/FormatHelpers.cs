using System;
using Vortice.Direct3D12;
using Vortice.DXGI;

namespace Engine.GPU
{
	public static class FormatHelpers
	{
		public static bool SupportsUAV(this Format format)
		{
			return !format.IsTypeless() && !format.IsDepthStencil() && !format.IsCompressed();
		}
	}
}
