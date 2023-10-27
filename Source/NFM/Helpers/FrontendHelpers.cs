using System.Runtime.ExceptionServices;
using Avalonia.Threading;

namespace NFM;

public static class FrontendHelpers
{
	/// <summary>
	/// Wraps a task to support the fancy exception handling UI. Should only use when run from UI thread.
	/// </summary>
	public static bool InvokeHandled(Action action)
	{
		try
		{
			action?.Invoke();
			return true;
		}
		catch (Exception e)
		{
			// Capture stack trace.
			ExceptionDispatchInfo info = ExceptionDispatchInfo.Capture(GetInnermost(e));

			// Create exception dialog.
			Dispatcher.UIThread.Post(() =>
			{
				new Dialog(
						info.SourceException.GetType().Name,
						$"An unhandled exception has occured. If you wish to debug this event further, select Break. Otherwise, select Abort to end the program.\n" +
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
		if (ex.InnerException == null || ex.InnerException == ex)
		{
			return ex;
		}
		else
		{
			return GetInnermost(ex.InnerException);
		}
	}
}
