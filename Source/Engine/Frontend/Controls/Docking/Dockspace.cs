using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;

namespace Engine.Frontend.Controls
{
	public enum DockPosition
	{
		Left = 0,
		Right = 1,
		Top = 2,
		Bottom = 3,
	}

	public struct DockRelationship
	{
		public DockRelationship() {}

		public TabGroup Parent = null;
		public DockPosition Direction = DockPosition.Right;
		public float Split = 0.5f; 
	}

	public class Dockspace : Panel
	{
		const int groupPadding = 6;

		public Dockspace()
		{
			Margin = new(groupPadding, 0, groupPadding, groupPadding);
		}

		public void Dock(TabGroup group, TabGroup parent, DockPosition direction = DockPosition.Right, float split = 0.5f)
		{
			// First child must be the central node (null parent).
			if (Children.Count == 0)
			{
				parent = null;
			}
			// ONLY first child can be central node.
			else
			{
				Debug.Assert(parent != null);
			}

			DockRelationship relationship = new()
			{
				Direction = direction,
				Parent = parent,
				Split = split,
			};

			group.Relationship = relationship;
			group.Dockspace = this;
			Children.Add(group);
		}

		protected override Size ArrangeOverride(Size FinalSize)
		{
			// Null parent means top-level - essentially parented to this dockspace. Only one group at a time can have a null parent.
			ArrangeRecursive(null, FinalSize.ToArea());

			// Apply calculated sizes.
			foreach (TabGroup Child in Children)
			{
				Child.Width = Child.CalculatedSize.Width;
				Child.Height = Child.CalculatedSize.Height;
				Child.Arrange(Child.CalculatedSize.ToRect());
			}

			return FinalSize;
		}

		private void ArrangeRecursive(TabGroup parent, Area parentSize)
		{
			// Loop through potential groups.
			foreach (TabGroup child in Children)
			{
				// This group is a child of the parent.
				if (child.Relationship.Parent == parent)
				{
					Area childSize = parentSize;

					if (parent != null)
					{
						switch (child.Relationship.Direction)
						{
							case DockPosition.Left:
								parentSize.Left += (child.Relationship.Split * parentSize.Width) + groupPadding;
								childSize.Right = parentSize.Left - groupPadding;
								break;

							case DockPosition.Right:
								parentSize.Right -= (child.Relationship.Split * parentSize.Width) + groupPadding;
								childSize.Left = parentSize.Right + groupPadding;
								break;

							case DockPosition.Top:
								parentSize.Top += (child.Relationship.Split * parentSize.Height) + groupPadding;
								childSize.Bottom = parentSize.Top - groupPadding;
								break;

							case DockPosition.Bottom:
								parentSize.Bottom -= (child.Relationship.Split * parentSize.Height) - groupPadding;
								childSize.Top = parentSize.Bottom + groupPadding;
								break;
						}
					}

					ArrangeRecursive(child, childSize);
				}
			}

			if (parent != null)
				parent.CalculatedSize = parentSize;
		}

		private static List<TabGroup> childGroups = new();
		public void CloseGroup(TabGroup target)
		{
			// Unless it's the only one present in the view.
			if (Children.Count == 1)
				return;

			int targetIndex = Children.IndexOf(target);
			Children.Remove(target);

			// Gather list of child groups.
			childGroups.Clear();
			foreach (TabGroup Group in Children)
			{
				if (Group.Relationship.Parent == target)
				{
					childGroups.Add(Group);
				}
			}

			TabGroup newParent = null;
			if (childGroups.Count > 0)
			{
				// Select new parent based on how visually jarring the change would be.
				int lastScore = 0;
				for (int i = 0; i < childGroups.Count; i++)
				{
					int groupScore = (int)childGroups[i].Relationship.Direction;

					if ((groupScore > lastScore) || (groupScore == lastScore && i > childGroups.IndexOf(newParent)))
					{
						lastScore = groupScore;
						newParent = childGroups[i];
					}
				}

				// Loop through orphans.
				foreach (TabGroup orphan in childGroups)
				{
					if (orphan == newParent)
					{
						continue;
					}

					// Reassign orphan to new parent.
					orphan.Relationship.Parent = newParent;
				}
			}

			if (newParent != null)
			{
				newParent.Relationship = target.Relationship;
				Children.Move(Children.IndexOf(newParent), targetIndex);
			}
		}
	}

	public static class RectExtensions
	{
		public static Rect ToRect(this Area input)
		{
			return new Rect(new Point(input.Left, input.Top), new Point(input.Right, input.Bottom));
		}

		public static Area ToArea(this Size input)
		{
			return new Area(0, (float)input.Width, 0, (float)input.Height);
		}
	}
}
