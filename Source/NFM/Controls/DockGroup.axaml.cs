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
using DynamicData;

namespace NFM;

public class DockGroup : TemplatedControl
{
	public static readonly StyledProperty<ObservableCollection<DockTab>> TabsProperty = AvaloniaProperty.Register<DockGroup, ObservableCollection<DockTab>>(nameof(Tabs));
	public static readonly StyledProperty<DockTab> SelectedTabProperty = AvaloniaProperty.Register<DockGroup, DockTab>(nameof(SelectedTab));

	public DockRelationship Relationship;
	public Box2D CalculatedSize;
	public DockSpace Dockspace;

	private int selectedIndex = -1;

	[Content]
	public ObservableCollection<DockTab> Tabs
	{
		get { return GetValue(TabsProperty); }
		internal set { SetValue(TabsProperty, value); }
	}

	public DockTab SelectedTab
	{
		get { return GetValue(SelectedTabProperty); }
		private set { SetValue(SelectedTabProperty, value); }
	}

	public DockGroup()
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

	public void Add<T>(T control) where T : ToolPanel
	{
		Tabs.Add(new DockTab(control, this));
	}

	public void CloseTab(DockTab tab)
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
