using System;

namespace NFM.Editor
{
	public interface ISelectable
	{
		string Name { get; }
	}

	public static class Selection
	{
		public static ReadOnlyObservableCollection<ISelectable> Selected { get; }
		private static ObservableRangeCollection<ISelectable> selected { get; } = new();

		static Selection()
		{
			Selected = new(selected);
		}

		public static void Select(params ISelectable[] items)
		{
			selected.AddRange(items);
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
}
