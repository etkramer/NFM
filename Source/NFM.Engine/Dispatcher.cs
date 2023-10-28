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
            TaskCompletionSource completionSource = new();
            dispatcherQueue.Enqueue(() =>
            {
                action.Invoke();
                completionSource.SetResult();
            });

            return completionSource.Task;
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
