using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using NFM.GPU;

namespace NFM.Threading;

public static class Dispatcher
{
    public static Thread MainThread { get; private set; } = Thread.CurrentThread;
    public static event Action<double> OnTick = delegate { };

    static readonly ConcurrentQueue<Action> dispatcherQueue = new();

    /// <summary>
    /// Schedules an action to be run on the main (dispatcher) thread.
    /// </summary>
    public static Task InvokeAsync(Action action)
    {
        var completionSource = new TaskCompletionSource();
        dispatcherQueue.Enqueue(() =>
        {
            action.Invoke();
            completionSource.SetResult();
        });

        return completionSource.Task;
    }

    /// <summary>
    /// Binds the dispatcher to the calling thread, which from here on is the main thread. Anything
    /// awaited there resumes through <see cref="Tick"/> rather than on a thread pool thread.
    /// </summary>
    internal static void Install()
    {
        MainThread = Thread.CurrentThread;
        SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext());
    }

    /// <summary>
    /// Throws if called anywhere except on the main thread.
    /// </summary>
    /// <param name="methodName"></param>
    /// <exception cref="Exception"></exception>
    [DebuggerHidden]
    public static void EnsureMainThread()
    {
        if (Thread.CurrentThread != MainThread)
        {
            throw new Exception($"Method called from wrong thread");
        }
    }

    internal static void Tick()
    {
        EnsureMainThread();

        while (dispatcherQueue.TryDequeue(out var action))
        {
            action.Invoke();
        }

        OnTick.Invoke(Metrics.FrameTime);
    }
}

/// <summary>
/// Marshals continuations back onto the main thread, where the engine expects all scene and GPU
/// work to happen. Loads still run wherever they were started - only their results come back here.
/// </summary>
sealed class DispatcherSynchronizationContext : SynchronizationContext
{
    public override void Post(SendOrPostCallback d, object? state) => Dispatcher.InvokeAsync(() => d.Invoke(state));
    public override SynchronizationContext CreateCopy() => this;
}
