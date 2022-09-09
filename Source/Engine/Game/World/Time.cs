using System;

namespace Engine
{
	public static class Time
	{
		/// <summary>
		/// Fires when the world time is changed.
		/// </summary>
		public static event Action<double> OnTimeChanged = delegate{};

		/// <summary>
		/// The current (world) time in seconds.
		/// </summary>
		public static double Now
		{
			get => now;
			set
			{
				now = value;
				OnTimeChanged(now);
			}
		}

		private static double now = 0d;
	}
}
