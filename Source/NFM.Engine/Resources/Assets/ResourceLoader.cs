namespace NFM.Resources;

/// <summary>
/// Describes a loader used when an unloaded asset is requested
/// </summary>
public abstract class ResourceLoader {}

/// <inheritdoc/>
public abstract class ResourceLoader<T> : ResourceLoader where T : GameResource
{
	public abstract Task<T> Load();
	public virtual void Unload() {}
}

public sealed class CachedResourceLoader<T> : ResourceLoader<T> where T : GameResource
{
    private T cachedValue;

    public CachedResourceLoader(T cachedValue)
    {
        this.cachedValue = cachedValue;
    }

    public override Task<T> Load() => Task.FromResult(cachedValue);
}