namespace NFM;

public interface ISelectable
{
	string Name { get; }

	void OnSelect();
	void OnDeselect();
}

public static class Selection
{
	public static ReadOnlyObservableCollection<ISelectable> Selected { get; }
	private static ObservableCollection<ISelectable> selected { get; } = new();

	static Selection()
	{
		Selected = new(selected);
	}

	public static void Select(ISelectable item)
	{
		selected.Add(item);
		item.OnSelect();
	}

	public static void Select(params ISelectable[] items) => Select(items);
	public static void Select(IEnumerable<ISelectable> items)
	{
		foreach (var item in items)
		{
			Select(item);
		}
	}

	public static void Deselect(ISelectable item)
	{
		selected.Remove(item);
		item.OnDeselect();
	}

	public static void Deselect(params ISelectable[] items) => Deselect(items);
	public static void Deselect(IEnumerable<ISelectable> items)
	{
		foreach (ISelectable item in items)
		{
			Deselect(item);
		}
	}

	public static void DeselectAll()
	{
		for (int i = selected.Count - 1; i >= 0; i--)
		{
			Deselect(selected[i]);
		}
	}
}
