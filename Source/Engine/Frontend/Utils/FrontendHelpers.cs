using System;
using System.Runtime.ExceptionServices;
using Avalonia.Threading;

namespace Engine.Frontend
{
	public static class FrontendHelpers
	{
		/// <summary>
		/// Wraps a task to support the fancy exception handling UI. Should only use when run from UI thread.
		/// </summary>
		public static void InvokeHandled(Action action)
		{
			try
			{
				action?.Invoke();
			}
			catch (Exception e)
			{
				// Capture stack trace.
				ExceptionDispatchInfo info = ExceptionDispatchInfo.Capture(e.InnerException);

				// Create exception dialog.
				Dispatcher.UIThread.Post(() =>
				{
					new Popup(
							info.SourceException.GetType().Name,
							$"An unhandled exception has occured. If you wish to debug this event further, select Break. Otherwise, select Abort to end the program.\n" +
							$"{info.SourceException.GetType().Name}: {info.SourceException.Message}\n" +
							$"{info.SourceException.StackTrace}")
						.Button("Break", () => info.Throw())
						.Button("Abort", () => Environment.Exit(-1)).Open();
				});
			};
		}
	}
}
