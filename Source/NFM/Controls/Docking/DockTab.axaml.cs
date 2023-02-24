using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Metadata;

namespace NFM;

[PseudoClasses(":active")]
public class DockTab : TemplatedControl
{
	public static readonly StyledProperty<ToolPanel> PanelProperty = AvaloniaProperty.Register<DockTab, ToolPanel>(nameof(Panel));
	public static readonly StyledProperty<bool> IsSelectedProperty = AvaloniaProperty.Register<DockTab, bool>(nameof(IsSelected), false);

	private DockGroup group;

	[Content]
	public ToolPanel Panel
	{
		get { return GetValue(PanelProperty); }
		set { SetValue(PanelProperty, value); }
	}

	public bool IsSelected
	{
		get { return GetValue(IsSelectedProperty); }
		set { SetValue(IsSelectedProperty, value); PseudoClasses.Set(":active", value); }
	}

	public DockTab(ToolPanel panel, DockGroup group)
	{
		this.group = group;
		Panel = panel;
		DataContext = this;
	}

	public void Close()
	{
		group.CloseTab(this);
	}

	public void OnClick()
	{
		// Switch group to this tab.
		group.ChangeSelection(group.Tabs.IndexOf(this));
	}
}
