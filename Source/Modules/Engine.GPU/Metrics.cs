using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.GPU
{
	public static class Metrics
	{
		/// <summary>
		/// The total amount of time it took to render the last frame (in seconds)
		/// </summary>
		public static double FrameTime { get; private set; }

		/// <summary>
		/// The current framerate (in FPS) - equivalent to (1 / Metrics.FrameTime)
		/// </summary>
		public static double FrameRate => 1 / FrameTime;

		/// <summary>
		/// The total number of frames that have been rendered since the program started.
		/// </summary>
		public static ulong FrameCount = 1;

		private static System.Diagnostics.Stopwatch frameTimer = new();

		internal static void BeginFrame()
		{
			// Update frame timer.
			frameTimer.Stop();
			FrameTime = frameTimer.Elapsed.TotalSeconds;
			frameTimer.Restart();

			FrameCount++;
		}
	}
}
