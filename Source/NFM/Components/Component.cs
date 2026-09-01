using Microsoft.AspNetCore.Components;

namespace NFM.Components;

/// <summary>
/// Base class for most Razor components. This differs from the built-in <see cref="ComponentBase"/> largely in its
/// ability to re-render based on external state changes.
/// </summary>
public abstract class Component : ComponentBase, IDisposable, IHandleEvent
{
	private static readonly List<Component> components = new();

	internal static void Tick()
	{
		for (int i = components.Count - 1; i >= 0; i--)
		{
			Component component = components[i];
			component.OnTick();

			int hashcode = component.BuildHashCode();
			if (hashcode != component.lastHashCode)
			{
				component.lastHashCode = hashcode;
				component.StateHasChanged();
			}
		}
	}

	/// <summary>
	/// Should this component automatically re-render every time an event callback is triggered?
	/// </summary>
	protected bool ShouldRenderOnEvent { get; init; }
	private int lastHashCode = -1;

	public Component() => components.Add(this);

	public void Dispose()
	{
		components.Remove(this);
		OnDispose();
		GC.SuppressFinalize(this);
	}

	protected virtual void OnDispose() {}
	protected virtual void OnTick() {}

	/// <summary>
	/// Override this to re-render whenever its return value changes. This can be
	/// combined with i.e. GetHashCode() to update based on changes in state.
	/// </summary>
	protected virtual int BuildHashCode() => -1;

	/// <summary>
	/// Prevents this component from re-rendering after every event.
	/// </summary>
	Task IHandleEvent.HandleEventAsync(EventCallbackWorkItem callback, object? arg)
	{
		Task task = callback.InvokeAsync(arg);
		bool shouldAwaitTask = task.Status != TaskStatus.RanToCompletion && task.Status != TaskStatus.Canceled;

		if (ShouldRenderOnEvent)
		{
			StateHasChanged();

			return shouldAwaitTask ? RenderOnCompletion(task) : Task.CompletedTask;
		}

		return shouldAwaitTask ? task : Task.CompletedTask;
	}

	/// <summary>
	/// Renders once an async handler finishes. Awaiting resumes on the dispatcher.
	/// </summary>
	private async Task RenderOnCompletion(Task task)
	{
		await task;
		await InvokeAsync(StateHasChanged);
	}
}
