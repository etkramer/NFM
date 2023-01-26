using System;

namespace NFM;

public interface ISelectable
{
	string Name { get; }
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
	}

	public static void Select(params ISelectable[] items)
	{
		foreach (var item in items)
		{
			Select(item);
		}
	}

	public static void Replace(params ISelectable[] items)
	{
		DeselectAll();
		Select(items);
	}

	public static void Deselect(params ISelectable[] items)
	{
		foreach (ISelectable item in items)
		{
			selected.Remove(item);
		}
	}

	public static void DeselectAll()
	{
		selected.Clear();
	}
}
