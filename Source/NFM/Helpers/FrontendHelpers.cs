using System.Runtime.ExceptionServices;

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
	/// Runs an action, reporting any exception through the fault dialog. UI thread only.
	/// </summary>
	public static bool InvokeHandled(Action action)
	{
		if (HasFaulted)
		{
			return false;
		}

		try
		{
			action.Invoke();
			return true;
		}
		catch (Exception e)
		{
			Report(e);
			return false;
		}
	}

	/// <summary>
	/// Runs an asynchronous action, reporting any exception through the fault dialog. UI thread only.
	/// </summary>
	public static async Task<bool> InvokeHandledAsync(Func<Task> action)
	{
		if (HasFaulted)
		{
			return false;
		}

		try
		{
			await action.Invoke();
			return true;
		}
		catch (Exception e)
		{
			Report(e);
			return false;
		}
	}

	/// <summary>
	/// Reports an exception through the fault dialog. UI thread only.
	/// </summary>
	public static void Report(Exception e)
	{
		HasFaulted = true;

		ExceptionDispatchInfo info = ExceptionDispatchInfo.Capture(GetInnermost(e));
		Exception source = info.SourceException;

		DialogResult result = MessageBox.Show(
			$"An unhandled exception has occured and the game loop has been stopped. Select Retry to break into the debugger, or Abort to end the program.\n\n" +
			$"{source.GetType().Name}: {source.Message}\n\n{source.StackTrace}",
			source.GetType().Name,
			MessageBoxButtons.AbortRetryIgnore,
			MessageBoxIcon.Error);

		if (result == DialogResult.Retry)
		{
			info.Throw();
		}
		else if (result == DialogResult.Abort)
		{
			Environment.Exit(-1);
		}
	}

	private static Exception GetInnermost(Exception e)
	{
		if (e.InnerException is null || e.InnerException == e)
		{
			return e;
		}

		return GetInnermost(e.InnerException);
	}
}
