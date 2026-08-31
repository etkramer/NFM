using Microsoft.AspNetCore.Components;

namespace NFM.Hosting;

/// <summary>
/// Marshals Blazor's renderer work onto the UI thread, which is also the thread the engine ticks on.
/// </summary>
sealed class FormDispatcher(Control control) : Dispatcher
{
	public override bool CheckAccess() => !control.InvokeRequired;

	public override Task InvokeAsync(Action workItem) => InvokeAsync(() => { workItem(); return Task.FromResult(0); });
	public override Task InvokeAsync(Func<Task> workItem) => InvokeAsync(async () => { await workItem(); return 0; });
	public override Task<TResult> InvokeAsync<TResult>(Func<TResult> workItem) => InvokeAsync(() => Task.FromResult(workItem()));

	public override async Task<TResult> InvokeAsync<TResult>(Func<Task<TResult>> workItem)
	{
		if (CheckAccess())
		{
			return await Run(workItem);
		}

		TaskCompletionSource<TResult> completion = new();

		control.BeginInvoke(async () =>
		{
			try
			{
				completion.SetResult(await Run(workItem));
			}
			catch (Exception e)
			{
				completion.SetException(e);
			}
		});

		return await completion.Task;
	}

	/// <summary>
	/// The renderer hands its unhandled exceptions back through here and discards the resulting task,
	/// so anything that faults would otherwise leave the page dead without a word.
	/// </summary>
	private static async Task<TResult> Run<TResult>(Func<Task<TResult>> workItem)
	{
		try
		{
			return await workItem();
		}
		catch (Exception e)
		{
			FrontendHelpers.Report(e);
			throw;
		}
	}
}
