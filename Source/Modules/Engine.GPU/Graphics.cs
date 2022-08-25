using System;
using Vortice.Direct3D12;
using Stopwatch = System.Diagnostics.Stopwatch;

namespace Engine.GPU
{
	public static class Graphics
	{
		/// <summary>
		/// The total number of frames that have been rendered
		/// </summary>
		public static ulong FrameCount => GPUContext.FrameCount;

		/// <summary>
		/// The time spent rendering the last frame, in seconds
		/// </summary>
		public static double FrameTime { get; private set; }

		public static event Action OnFrameStart = delegate {};

		private static CommandList commandList;
		public static CommandList GetCommandList()
		{
			if (commandList == null)
			{
				commandList = new();
			}

			return commandList;
		}

		private static Stopwatch frameTimer = new();

		public static void SubmitAndWait()
		{
			// Execute default command list.
			GetCommandList().Execute();
			
			// Wait for completion.
			GPUContext.WaitFrame();
			OnFrameStart.Invoke();

			// Reopen default command list.
			GetCommandList().Reset();

			// Update frame timer.
			frameTimer.Stop();
			FrameTime = frameTimer.Elapsed.TotalSeconds;
			frameTimer.Restart();
		}

		public static void Flush()
		{
			GPUContext.WaitIdle();
		}
	}
}
