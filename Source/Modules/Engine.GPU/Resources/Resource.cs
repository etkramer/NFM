using System;
using Vortice.Direct3D12;

namespace Engine.GPU
{
	public abstract class Resource
	{
		internal ResourceStates State;
		internal abstract ID3D12Resource GetBaseResource();

		public static implicit operator ID3D12Resource(Resource res)
		{
			return res.GetBaseResource();
		}
	}
}
