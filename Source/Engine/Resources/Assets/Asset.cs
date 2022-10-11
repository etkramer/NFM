using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace Engine.Resources
{
	public abstract class Asset
	{
		public static event Action<Asset> OnAssetAdded = delegate {};

		public string Path { get; set; }
		public string Name { get; protected set;}
		public static readonly ConcurrentDictionary<string, Asset> Assets = new(StringComparer.OrdinalIgnoreCase);

		/// <summary>
		/// Submits the given asset to the asset system
		/// </summary>
		public static bool Submit<T>(Asset<T> asset) where T : Resource
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
		public static Task<T> LoadAsync<T>(string path) where T : Resource
		{
			if (Assets.TryGetValue(path, out Asset foundAsset))
			{
				if (foundAsset is Asset<T> asset)
				{
					return asset.GetAsync();
				}
			}

			return Task.FromResult<T>(null);
		}
	}

	public sealed class Asset<T> : Asset where T : Resource
	{
		private object loadingLock = new();
		private Task<T> loadingTask;

		private readonly ResourceLoader<T> loader;
		private T cache;

		public bool IsLoaded => cache != null;

		public Asset(string path, MountPoint mount, ResourceLoader<T> loader)
		{
			Path = mount.MakeFullPath(path);
			Name = Path.Split('/').Last();
			this.loader = loader;
		}

		public Asset(string path, MountPoint mount, T cachedValue)
		{
			Path = mount.MakeFullPath(path);
			Name = Path.Split('/').Last();
			cache = cachedValue;
			cache.Source = this;
			loader = null;
		}

		public Task<T> GetAsync()
		{
			lock (loadingLock)
			{
				// Don't start a new task if we've already started (or finished) loading it.
				if (loadingTask == null)
				{
					// Resource is not loaded, so we need to load it.
					if (cache == null)
					{
						loadingTask = Task.Run(async () =>
						{
							cache = await loader.Load();
							cache.Source = this;

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
}