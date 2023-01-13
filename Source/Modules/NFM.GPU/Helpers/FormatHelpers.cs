using System;
using Vortice.Direct3D12;
using Vortice.DXGI;

namespace NFM.GPU
{
	public static class FormatHelpers
	{
		public static bool SupportsUAV(this Format format)
		{
			return !format.IsTypeless() && !format.IsDepthStencil() && !format.IsCompressed();
		}

		public static bool SupportsRTV(this Format format)
		{
			return !format.IsTypeless() && !format.IsDepthStencil() && !format.IsCompressed();
		}

		public static bool SupportsDSV(this Format format)
		{
			return (format.IsTypeless() || format.IsDepthStencil()) && !format.IsCompressed();
		}
	}
}
