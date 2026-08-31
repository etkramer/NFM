namespace NFM.Components;

public sealed partial class TreeView<TItem> where TItem : class
{
	/// <summary>
	/// Flat list of currently displayed nodes, not accounting for virtualization.
	/// </summary>
	private readonly List<TreeNode> displayNodes = new();

	/// <summary>
	/// Item/node mappings. Includes non-visible/collapsed items that aren't in displayNodes.
	/// </summary>
	private readonly Dictionary<TItem, TreeNode> nodeItemMap = new();

	private IEnumerable<TItem>? lastRootItems;

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

			foreach (TItem item in RootItems)
			{
				AddItem(item);
				displayNodes.Add(nodeItemMap[item]);
			}
		}

		return base.OnParametersSetAsync();
	}

	// Scene contents are built on a loader thread, so changes have to be marshalled before the tree touches them.
	private void OnItemsChanged(object? sender, NotifyCollectionChangedEventArgs args) => _ = InvokeAsync(() => ApplyItemsChanged(args));
	private void OnItemChildrenChanged(TreeNode node) => _ = InvokeAsync(() => ApplyItemChildrenChanged(node));

	private void ApplyItemsChanged(NotifyCollectionChangedEventArgs args)
	{
		if (args.OldItems is not null)
		{
			foreach (TItem oldItem in args.OldItems)
			{
				RemoveItem(oldItem);
			}
		}

		if (args.NewItems is not null)
		{
			foreach (TItem newItem in args.NewItems)
			{
				AddItem(newItem);
				displayNodes.Add(nodeItemMap[newItem]);
			}
		}

		StateHasChanged();
	}

	/// <summary>
	/// Creates the node, but does not display it in the tree.
	/// </summary>
	private void AddItem(TItem item)
	{
		if (nodeItemMap.ContainsKey(item))
		{
			return;
		}

		TreeNode node = new(item, GetChildren.Invoke(item), OnItemChildrenChanged);
		nodeItemMap.Add(item, node);

		foreach (TItem child in node.Children)
		{
			AddItem(child);
		}
	}

	/// <summary>
	/// Removes the node, and its place in the tree.
	/// </summary>
	private void RemoveItem(TItem item)
	{
		if (!nodeItemMap.TryGetValue(item, out TreeNode? node))
		{
			return;
		}

		foreach (TItem child in node.Children.ToArray())
		{
			RemoveItem(child);
		}

		node.Dispose();
		nodeItemMap.Remove(item);
		displayNodes.Remove(node);
	}

	/// <summary>
	/// Displays children of this node in the tree.
	/// </summary>
	private void ExpandNode(TreeNode node)
	{
		node.IsExpanded = true;
		int parentIndex = displayNodes.IndexOf(node);

		for (int i = 0; i < node.Children.Count; i++)
		{
			TreeNode childNode = nodeItemMap[node.Children[i]];
			childNode.IndentLevel = node.IndentLevel + 1;
			displayNodes.Insert(parentIndex + i + 1, childNode);

			if (childNode.IsExpanded)
			{
				ExpandNode(childNode);
			}
		}
	}

	/// <summary>
	/// Hides children of this node from the tree.
	/// </summary>
	private void CollapseNode(TreeNode node, bool isRecursing = false)
	{
		if (!node.IsExpanded)
		{
			return;
		}

		// Don't override isExpanded state of child nodes
		if (!isRecursing)
		{
			node.IsExpanded = false;
		}

		foreach (TItem child in node.Children)
		{
			CollapseNode(nodeItemMap[child], true);
			displayNodes.Remove(nodeItemMap[child]);
		}
	}

	private void ApplyItemChildrenChanged(TreeNode node)
	{
		node.Refresh();

		foreach (TItem child in node.Children)
		{
			AddItem(child);
		}

		if (node.IsExpanded)
		{
			CollapseNode(node);
			ExpandNode(node);
		}

		StateHasChanged();
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
			Children = children.ToList();
			handler = (o, e) => onChildrenChanged.Invoke(this);

			if (source is INotifyCollectionChanged notify)
			{
				notify.CollectionChanged += handler;
			}
		}

		public void Refresh() => Children = source.ToList();

		public void Dispose()
		{
			if (source is INotifyCollectionChanged notify)
			{
				notify.CollectionChanged -= handler;
			}
		}
	}
}
