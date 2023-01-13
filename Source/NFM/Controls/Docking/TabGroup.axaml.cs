using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Metadata;

namespace NFM.Frontend
{
	public class TabGroup : TemplatedControl
	{
		public static readonly StyledProperty<ObservableCollection<Tab>> TabsProperty = AvaloniaProperty.Register<TabGroup, ObservableCollection<Tab>>(nameof(Tabs));
		public static readonly StyledProperty<Tab> SelectedTabProperty = AvaloniaProperty.Register<TabGroup, Tab>(nameof(SelectedTab));

		public DockRelationship Relationship;
		public Box2D CalculatedSize;
		public Dockspace Dockspace;

		private int selectedIndex = -1;

		[Content]
		public ObservableCollection<Tab> Tabs
		{
			get { return GetValue(TabsProperty); }
			internal set { SetValue(TabsProperty, value); }
		}

		public Tab SelectedTab
		{
			get { return GetValue(SelectedTabProperty); }
			private set { SetValue(SelectedTabProperty, value); }
		}

		public TabGroup()
		{
			Tabs = new();
		}

		protected override void OnApplyTemplate(TemplateAppliedEventArgs Event)
		{
			ChangeSelection(0);
		}

		public void ChangeSelection(int index)
		{
			if (index != -1)
			{
				if (SelectedTab != null)
				{
					SelectedTab.IsSelected = false;
				}

				selectedIndex = index;
				UpdateSelectedContent();
				SelectedTab.IsSelected = true;
			}
		}

		public void CloseTab(Tab tab)
		{
			// Remove single tab.
			if (Tabs.Count > 1)
			{
				Tabs.Remove(tab);

				if (selectedIndex > Tabs.Count - 1)
				{
					ChangeSelection(selectedIndex - 1);
				}
				else
				{
					ChangeSelection(selectedIndex);
				}
			}
			// Remove entire group.
			else
			{
				Dockspace.CloseGroup(this);
			}
		}

		private void UpdateSelectedContent()
		{
			if (Tabs.Count > selectedIndex)
			{
				SelectedTab = Tabs[selectedIndex];
			}
		}
	}
}
