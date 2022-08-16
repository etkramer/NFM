using System;
using Vortice.Direct3D12;

namespace Engine.GPU
{
	public struct CommandInput
	{
		public Resource Resource;
		public ResourceStates State;

		public CommandInput(Resource resource, ResourceStates state)
		{
			Resource = resource;
			State = state;
		}
	}
}
