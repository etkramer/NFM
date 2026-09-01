using NFM.Resources;

namespace NFM.Components;

/// <summary>
/// A folder in the asset path hierarchy. Mount roots are the folders without a parent.
/// </summary>
public sealed class LibraryFolder
{
	public string Name { get; }
	public string Path { get; }
	public LibraryFolder? Parent { get; }

	public ObservableCollection<LibraryFolder> Children { get; } = [];
	public ObservableCollection<Asset> Assets { get; } = [];

	public LibraryFolder(string name, string path, LibraryFolder? parent)
	{
		Name = name;
		Path = path;
		Parent = parent;
	}

	/// <summary>
	/// Number of assets in this folder and everything below it.
	/// </summary>
	public int TotalCount => Assets.Count + Children.Sum(o => o.TotalCount);

	public IEnumerable<Asset> AllAssets => Assets.Concat(Children.SelectMany(o => o.AllAssets));

	/// <summary>
	/// Gets the named subfolder, creating it if it doesn't exist yet.
	/// </summary>
	public LibraryFolder GetChild(string name)
	{
		foreach (LibraryFolder child in Children)
		{
			if (child.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
			{
				return child;
			}
		}

		LibraryFolder folder = new(name, $"{Path}/{name}", this);
		Children.Insert(SortedIndexOf(Children, o => o.Name, name), folder);

		return folder;
	}

	public void Add(Asset asset) => Assets.Insert(SortedIndexOf(Assets, o => o.Name, asset.Name), asset);

	static int SortedIndexOf<T>(IEnumerable<T> items, Func<T, string> nameOf, string name) =>
		items.TakeWhile(o => string.Compare(nameOf(o), name, StringComparison.OrdinalIgnoreCase) < 0).Count();
}
