namespace NFM.Resources;

public abstract class GameResource : IDisposable
{
	internal Asset? Source { get; set; } = null;

    /// <summary>
    /// False until after PostLoad() is called.
    /// </summary>
    public bool IsFullyLoaded { get; internal set; } = false;

    /// <summary>
    /// Called by the engine when the resource is first used, guaranteed to run on the main thread.
    /// </summary>
    protected virtual void PostLoad() { }

    /// <summary>
    /// Fully loads a resource if it hasn't been already, otherwise does nothing.
    /// </summary>
    public void EnsureFullyLoaded()
    {
        Dispatcher.EnsureMainThread();

        if (!IsFullyLoaded)
        {
            PostLoad();
            IsFullyLoaded = true;
        }
    }

	public virtual void Dispose() { }
}
