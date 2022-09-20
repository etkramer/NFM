using System;
using Vortice.Direct3D12;

namespace Engine.GPU
{
	public abstract class Resource
	{
		internal ResourceStates State;
		internal abstract ID3D12Resource D3DResource { get; private protected set; }
		public bool IsAlive { get; protected set; } = true;

		public static implicit operator ID3D12Resource(Resource res)
		{
			return res.D3DResource;
		}
	}
}
