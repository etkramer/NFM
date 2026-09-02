using System.Collections.Concurrent;
using NFM.Resources;

namespace NFM.Components;

/// <summary>
/// Folder hierarchy over the flat asset registry, grouped by mount point. Assets are submitted from
/// loader threads, so new ones are queued and folded in by <see cref="Refresh"/> on the main thread.
/// </summary>
public static class AssetTree
{
	public static ObservableCollection<LibraryFolder> Roots { get; } = [];

	/// <summary>
	/// Bumped whenever the tree changes, for components that poll rather than observe.
	/// </summary>
	public static int Version { get; private set; }

	static readonly ConcurrentQueue<Asset> pending = new();
	static readonly HashSet<string> known = new(StringComparer.OrdinalIgnoreCase);

	static AssetTree()
	{
		Asset.OnAssetAdded += pending.Enqueue;

		foreach (Asset asset in Asset.Assets.Values)
		{
			pending.Enqueue(asset);
		}
	}

	public static IEnumerable<Asset> AllAssets => Roots.SelectMany(o => o.AllAssets);

	/// <summary>
	/// Folds every asset submitted since the last call into the hierarchy.
	/// </summary>
	public static void Refresh()
	{
		bool changed = false;

		while (pending.TryDequeue(out Asset? asset))
		{
			changed |= Insert(asset);
		}

		if (changed)
		{
			Version++;
		}
	}

	static bool Insert(Asset asset)
	{
		if (!known.Add(asset.Path))
		{
			return false;
		}

		// "USER:/Models/Foo.glb" splits into the mount ID, any folders, then the asset itself.
		string[] parts = asset.Path.Split('/');
		LibraryFolder folder = GetRoot(parts[0].TrimEnd(':'));

		for (int i = 1; i < parts.Length - 1; i++)
		{
			folder = folder.GetChild(parts[i]);
		}

		folder.Add(asset);
		return true;
	}

	static LibraryFolder GetRoot(string id)
	{
		string path = $"{id}:";

		foreach (LibraryFolder root in Roots)
		{
			if (root.Path.Equals(path, StringComparison.OrdinalIgnoreCase))
			{
				return root;
			}
		}

		LibraryFolder folder = new(FindMountName(id) ?? id, path, null);
		Roots.Add(folder);

		return folder;
	}

	// Scanned newest-first, so the last mount to claim an ID is the one that names it.
	static string? FindMountName(string id)
	{
		IReadOnlyList<MountPoint> mounts = MountPoint.All;

		for (int i = mounts.Count - 1; i >= 0; i--)
		{
			if (mounts[i].ID.Equals(id, StringComparison.OrdinalIgnoreCase))
			{
				return mounts[i].Name;
			}
		}

		return null;
	}
}
