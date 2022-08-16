using System;
using Vortice.Direct3D12;

namespace Engine.GPU
{
	public class CommandAllocator : IDisposable
	{
		internal ID3D12CommandAllocator[] commandAllocators;

		public CommandAllocator(CommandListType type = CommandListType.Direct)
		{
			// Create command allocators.
			commandAllocators = new ID3D12CommandAllocator[GPUContext.RenderLatency];
			for (int i = 0; i < GPUContext.RenderLatency; i++)
			{
				commandAllocators[i] = GPUContext.Device.CreateCommandAllocator(type);
			}
		}

		public void Reset()
		{
			commandAllocators[GPUContext.FrameIndex].Reset();
		}

		public void Dispose()
		{
			for (int i = 0; i < commandAllocators.Length; i++)
			{
				commandAllocators[i].Dispose();
			}
		}
	}
}
