using System.Collections.Concurrent;
using NFM.Threading;

namespace NFM.Resources;

public abstract class Asset
{
	public static readonly ConcurrentDictionary<string, Asset> Assets = new(StringComparer.OrdinalIgnoreCase);
	public static event Action<Asset> OnAssetAdded = delegate {};

	// TODO: Should try to get thumbnail from loader, and generate (or load from cache) a new one if it returns null.
	// This is for i.e. a cloud-based asset provider might want to send along just the thumbnail without needing the asset to be (down)loaded.
	public Texture2D? Thumbnail => null;

	public string Path { get; set; }
	public string Name => Path.Split('/').Last();

    protected internal Asset(string path) { Path = path; }

	/// <summary>
	/// Submits the given asset to the asset system
	/// </summary>
	public static bool Submit<T>(Asset<T> asset) where T : GameResource
	{
		if (Assets.TryAdd(asset.Path, asset))
		{
			OnAssetAdded.Invoke(asset);
			return true;
		}

		return false;
	}

	/// <summary>
	/// Asynchronously retrieves the asset at the given path
	/// </summary>
	public static async Task<T?> LoadAsync<T>(string path) where T : GameResource
	{
		if (Assets.TryGetValue(path, out Asset? foundAsset))
		{
			if (foundAsset is Asset<T> asset)
			{
				return await asset.GetAsync();
			}
		}

		return null;
	}
}

public sealed class Asset<T> : Asset where T : GameResource
{
	private object loadingLock = new();
	private Task<T>? loadingTask;

	private readonly ResourceLoader<T> loader;
	private T? cache;

	public bool IsLoaded => cache is not null;

	public Asset(string path, MountPoint mount, ResourceLoader<T> loader) : base(mount.MakeFullPath(path))
	{
		this.loader = loader;
	}

	public Task<T> GetAsync()
	{
		lock (loadingLock)
		{
			// Don't start a new task if we've already started (or finished) loading it.
			if (loadingTask is null)
			{
				// Resource is not loaded, so we need to load it.
				if (cache is null)
				{
					loadingTask = Task.Run(async () =>
					{
                        // Run loader function
						cache = await Guard.NotNull(loader).Load();
                        cache.Source = this;

                        // Schedule upload to occur on the main thread
                        await Dispatcher.InvokeAsync(cache.PostLoad);
                        cache.IsFullyLoaded = true;

						return cache;
					});
				}
				// Resource is already loaded, so we can just return it.
				else
				{
					loadingTask = Task.FromResult(cache);
				}
			}
		}

		return loadingTask;
	}
}