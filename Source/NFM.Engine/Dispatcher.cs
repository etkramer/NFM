using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using NFM.GPU;

namespace NFM.Threading;

public static class Dispatcher
{
    public static Thread MainThread { get; private set; } = Thread.CurrentThread;
    public static event Action<double> OnTick = delegate { };

    static Queue<Action> dispatcherQueue = new();

    /// <summary>
    /// Schedules an action to be run on the main (dispatcher) thread.
    /// </summary>
    public static Task InvokeAsync(Action action)
    {
        lock (dispatcherQueue)
        {
            var completionSource = new TaskCompletionSource();
            dispatcherQueue.Enqueue(() =>
            {
                action.Invoke();
                completionSource.SetResult();
            });

            return completionSource.Task;
        }
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
        if (MainThread != Thread.CurrentThread)
        {
            MainThread = Thread.CurrentThread;
        }

        lock (dispatcherQueue)
        {
            while (dispatcherQueue.TryDequeue(out var action))
            {
                action.Invoke();
            }
        }

        OnTick.Invoke(Metrics.FrameTime);
    }
}
