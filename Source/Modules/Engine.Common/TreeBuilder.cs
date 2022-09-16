using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Engine.Common
{
	public class TreeNode<T>
	{
		public T Item { get; private set; }
		public ObservableCollection<TreeNode<T>> Children { get; private set; } = new();
		public TreeNode<T> Parent { get; private set; }

		public TreeNode(T item)
		{
			Item = item;
		}
	}

	public class TreeBuilder<T> : IEnumerable<T>, INotifyCollectionChanged
	{
		private TreeNode<T> rootNode = new(default);
		private TreeNode<T> currentNode;

		public event NotifyCollectionChangedEventHandler CollectionChanged
		{
			add
			{
				(rootNode.Children as INotifyCollectionChanged).CollectionChanged += value;
			}
			remove
			{
				(rootNode.Children as INotifyCollectionChanged).CollectionChanged -= value;
			}
		}

		public TreeBuilder()
		{
			currentNode = rootNode;
		}

		public bool AddNode(T value, bool select = false)
		{
			if (!currentNode.Children.Any(o => o.Item.Equals(value)))
			{
				TreeNode<T> node = new TreeNode<T>(value);
				currentNode.Children.Add(node);

				if (select)
				{
					currentNode = node;
				}

				return true;
			}

			return false;
		}

		public IEnumerator<T> GetEnumerator()
		{
			return (rootNode.Children as IEnumerable<T>).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return (rootNode.Children as IEnumerable).GetEnumerator();
		}

		public bool SelectNode(T value)
		{
			// It's a direct child, simple enough.
			if (currentNode.Children.TryFirst(o => o.Item.Equals(value), out TreeNode<T> result))
			{
				currentNode = result;
				return true;
			}
			// Search through every node in the tree - obviously much slower, should be avoided if possible.
			else
			{
				bool selectNodeRecursive(T searchTarget, TreeNode<T> root, out TreeNode<T> result)
				{
					if (root.Children.TryFirst(o => o.Item.Equals(searchTarget), out result))
					{
						return true;
					}
					else
					{
						foreach (var child in root.Children)
						{
							if (selectNodeRecursive(searchTarget, child, out result))
							{
								return true;
							}
						}
					}

					return false;
				}

				if (selectNodeRecursive(value, rootNode, out TreeNode<T> selectedNode))
				{
					currentNode = selectedNode;
					return true;
				}
			}

			return false;
		}

		public bool SelectParent()
		{
			if (currentNode.Parent != null)
			{
				currentNode = currentNode.Parent;
				return true;
			}
			else
			{
				return false;
			}
		}

		public void SelectRoot()
		{
			currentNode = rootNode;
		}
	}
}
