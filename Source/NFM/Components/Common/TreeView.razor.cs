using Microsoft.AspNetCore.Components.Web;

namespace NFM.Components;

public sealed partial class TreeView<TItem> where TItem : class
{
	/// <summary>
	/// Flat list of currently displayed nodes, not accounting for virtualization.
	/// </summary>
	private readonly List<TreeNode> displayNodes = [];

	/// <summary>
	/// Item/node mappings. Includes non-visible/collapsed items that aren't in displayNodes.
	/// </summary>
	private readonly Dictionary<TItem, TreeNode> nodeItemMap = [];

	private readonly HashSet<TItem> reachable = [];

	private IEnumerable<TItem>? lastRootItems;

	/// <summary>
	/// Item currently being dragged, if any.
	/// </summary>
	public TItem? DraggedItem => DragDrop.Payload as TItem;

	private TItem? dropTargetItem;

	protected override Task OnParametersSetAsync()
	{
		if (RootItems != lastRootItems)
		{
			foreach (TreeNode node in nodeItemMap.Values)
			{
				node.Dispose();
			}

			displayNodes.Clear();
			nodeItemMap.Clear();

			if (lastRootItems is INotifyCollectionChanged oldNotify)
			{
				oldNotify.CollectionChanged -= OnItemsChanged;
			}
			if (RootItems is INotifyCollectionChanged newNotify)
			{
				newNotify.CollectionChanged += OnItemsChanged;
			}

			lastRootItems = RootItems;
			Rebuild();
		}

		return base.OnParametersSetAsync();
	}

	// Scene contents are built on a loader thread, so changes have to be marshalled before the tree touches them.
	private void OnItemsChanged(object? sender, NotifyCollectionChangedEventArgs args) => _ = InvokeAsync(ApplyChanged);
	private void OnItemChildrenChanged(TreeNode node) => _ = InvokeAsync(ApplyChanged);

	private void ApplyChanged()
	{
		Rebuild();
		StateHasChanged();
	}

	/// <summary>
	/// Rebuilds the flat display list from the current item hierarchy, preserving expansion state.
	/// Items can move between parents in either order, so this is done wholesale rather than in-place.
	/// </summary>
	private void Rebuild()
	{
		displayNodes.Clear();
		reachable.Clear();

		foreach (TItem item in RootItems)
		{
			AddItem(item, 0, true);
		}

		foreach (TItem item in nodeItemMap.Keys.ToArray())
		{
			if (!reachable.Contains(item))
			{
				nodeItemMap[item].Dispose();
				nodeItemMap.Remove(item);
			}
		}
	}

	private void AddItem(TItem item, int indentLevel, bool isVisible)
	{
		if (!reachable.Add(item))
		{
			return;
		}

		if (!nodeItemMap.TryGetValue(item, out TreeNode? node))
		{
			node = new(item, GetChildren.Invoke(item), OnItemChildrenChanged);
			nodeItemMap.Add(item, node);
		}

		node.Refresh();

		if (isVisible)
		{
			node.IndentLevel = indentLevel;
			displayNodes.Add(node);
		}

		foreach (TItem child in node.Children)
		{
			AddItem(child, indentLevel + 1, isVisible && node.IsExpanded);
		}
	}

	public bool GetIsDropTarget(TItem item) => DraggedItem is not null && dropTargetItem == item;

	public void OnPointerDownItem(TItem item, PointerEventArgs args)
	{
		if (IsDraggable)
		{
			DragDrop.Arm(item, args);
		}
	}

	public void OnPointerMoveItem(TItem? item, PointerEventArgs args)
	{
		TItem? dragged = DragDrop.Update(args) as TItem;
		dropTargetItem = dragged is not null && CanDrop.Invoke(dragged, item) ? item : null;
	}

	public void OnPointerUpItem(TItem item, PointerEventArgs args) => OnDropOnItem(item);

	/// <summary>
	/// Completes the active drag onto the given target, or onto the root when it's null.
	/// </summary>
	public void OnDropOnItem(TItem? target)
	{
		if (DraggedItem is TItem dragged && CanDrop.Invoke(dragged, target))
		{
			OnDrop.Invoke(dragged, target);
		}

		DragDrop.Clear();
		dropTargetItem = null;
	}

	public sealed class TreeNode : IDisposable
	{
		public TItem Item { get; }
		public List<TItem> Children { get; private set; }

		public int IndentLevel { get; set; }
		public bool IsExpanded { get; set; }

		private readonly IEnumerable<TItem> source;
		private readonly NotifyCollectionChangedEventHandler handler;

		public TreeNode(TItem item, IEnumerable<TItem> children, Action<TreeNode> onChildrenChanged)
		{
			Item = item;
			source = children;
			Children = [.. children];
			handler = (o, e) => onChildrenChanged.Invoke(this);

			if (source is INotifyCollectionChanged notify)
			{
				notify.CollectionChanged += handler;
			}
		}

		public void Refresh() => Children = [.. source];

		public void Dispose()
		{
			if (source is INotifyCollectionChanged notify)
			{
				notify.CollectionChanged -= handler;
			}
		}
	}
}
