using System.Runtime.ExceptionServices;
using Avalonia.Threading;

namespace NFM;

public static class FrontendHelpers
{
	/// <summary>
	/// True once an unhandled exception has been reported. Nothing else runs through
	/// <see cref="InvokeHandled"/> afterwards, so a fault in a per-frame callback reports once
	/// instead of once per frame.
	/// </summary>
	public static bool HasFaulted { get; private set; } = false;

	/// <summary>
	/// Wraps a task to support the fancy exception handling UI. Should only use when run from UI thread.
	/// </summary>
	public static bool InvokeHandled(Action action)
	{
		if (HasFaulted)
		{
			return false;
		}

		try
		{
			action?.Invoke();
			return true;
		}
		catch (Exception e)
		{
			HasFaulted = true;

			// Capture stack trace.
			ExceptionDispatchInfo info = ExceptionDispatchInfo.Capture(GetInnermost(e));

			// Create exception dialog.
			Dispatcher.UIThread.Post(() =>
			{
				new Dialog(
						info.SourceException.GetType().Name,
						$"An unhandled exception has occured and the game loop has been stopped. If you wish to debug this event further, select Break. Otherwise, select Abort to end the program.\n" +
						$"{info.SourceException.GetType().Name}: {info.SourceException.Message}\n" +
						$"{info.SourceException.StackTrace}")
					.Button("Break", (o) => info.Throw())
					.Button("Abort", (o) => Environment.Exit(-1)).Show();
			});

			return false;
		};
	}

	private static Exception GetInnermost(Exception ex)
	{
		if (ex.InnerException is null || ex.InnerException == ex)
		{
			return ex;
		}
		else
		{
			return GetInnermost(ex.InnerException);
		}
	}
}
