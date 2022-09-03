using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace Engine.Resources
{
	public abstract class Asset
	{
		public string Path { get; set; }

		private static readonly ConcurrentDictionary<string, Asset> assets = new(StringComparer.OrdinalIgnoreCase);

		/// <summary>
		/// Submits the given asset to the asset system
		/// </summary>
		public static bool Submit<T>(Asset<T> asset) where T : Resource
		{
			return assets.TryAdd(asset.Path, asset);
		}

		/// <summary>
		/// Asynchronously retrieves the asset at the given path
		/// </summary>
		public static Task<T> GetAsync<T>(string path) where T : Resource
		{
			if (assets.TryGetValue(path, out Asset foundAsset))
			{
				if (foundAsset is Asset<T> asset)
				{
					return asset.Get();
				}
			}

			return Task.FromResult<T>(null);
		}
	}

	public sealed class Asset<T> : Asset where T : Resource
	{
		private Task<T> loadingTask;
		private readonly AssetLoader<T> loader;
		private T cache;

		public Asset(string path, AssetPrefix prefix, AssetLoader<T> loader)
		{
			Path = prefix.MakeFullPath(path);
			this.loader = loader;
		}

		public Asset(string path, AssetPrefix prefix, T cachedValue)
		{
			Path = prefix.MakeFullPath(path);
			cache = cachedValue;
			loader = null;

			if (!cache.IsLoaded)
			{
				cache.OnLoad();
				cache.Source = this;
				cache.IsLoaded = true;
			}
		}

		public Task<T> Get()
		{
			if (cache == null)
			{
				// Don't start a new task if we're already in the process of loading it.
				if (loadingTask == null)
				{
					loadingTask = Task.Run(async () =>
					{
						cache = await loader.Load();
						if (!cache.IsLoaded)
						{
							cache.OnLoad();
							cache.Source = this;
							cache.IsLoaded = true;
						}

						return cache;
					});
				}

				return loadingTask;
			}
			else
			{
				return Task.FromResult(cache);
			}
		}
	}
}