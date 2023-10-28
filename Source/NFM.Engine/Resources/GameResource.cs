namespace NFM.Resources;

public abstract class GameResource : IDisposable
{
	internal Asset? Source { get; set; } = null;

    /// <summary>
    /// False until after PostLoad() is called.
    /// </summary>
    public bool IsFullyLoaded { get; internal set; } = false;

    /// <summary>
    /// Called by the asset system when the resource has finished loading, guaranteed to run on the main thread.
    /// </summary>
    protected internal virtual void PostLoad() { }

	public virtual void Dispose() { }
}
