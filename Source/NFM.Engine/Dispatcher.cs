using NFM.GPU;

namespace NFM.Threading;

public static class Dispatcher
{
    public static event Action<double> OnTick = delegate {};

    static Queue<Action> dispatcherQueue = new();

    public static Task InvokeAsync(Action action)
    {
        lock(dispatcherQueue)
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
        while (dispatcherQueue.TryDequeue(out var action))
        {
            action.Invoke();
        };

        OnTick.Invoke(Metrics.FrameTime);
    }
}
