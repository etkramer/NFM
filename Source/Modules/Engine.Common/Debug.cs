using System;

namespace Engine.Common
{
	public class AssertionFailedException : Exception
	{
		public AssertionFailedException(string message = null) : base(message) {}
	}

	public static class Debug
	{
		#if DEBUG
		public const bool IsDebugBuild = true;
		#else
		public const bool IsDebugBuild = false;
		#endif

		public static readonly bool IsDebugMode;

		static Debug()
		{
			IsDebugMode = IsDebugBuild && System.Diagnostics.Debugger.IsAttached;
		}

		private static void LogBase(object message)
		{
			Console.WriteLine(message);
		}

		public static void Log(object message)
		{
			LogBase($"INFO: {message}");
		}

		public static void LogWarning(object message)
		{
			LogBase($"WARNING: {message}");
		}

		public static void LogError(object message)
		{
			LogBase($"ERROR: {message}");
		}

		public static void LogError(Exception ex)
		{
			LogBase($"ERROR: {ex.GetType().Name}: {ex.Message}");
		}

		public static void Throw(string message)
		{
			throw new Exception(message);
		}

		public static void Assert(bool condition, string message = null)
		{
			if (!condition)
			{
				if (message == null)
				{
					throw new AssertionFailedException();
				}
				else
				{
					throw new AssertionFailedException(message);
				}
			}
		}
	}
}
