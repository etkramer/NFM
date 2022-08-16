using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Metadata;

namespace Engine.Frontend.Controls
{
	[PseudoClasses(":active")]
	public class Tab : TemplatedControl
	{
		public static readonly StyledProperty<ToolWindow> PanelProperty = AvaloniaProperty.Register<Tab, ToolWindow>(nameof(Panel));
		public static readonly StyledProperty<bool> IsSelectedProperty = AvaloniaProperty.Register<Tab, bool>(nameof(IsSelected), false);

		private TabGroup group;

		[Content]
		public ToolWindow Panel
		{
			get { return GetValue(PanelProperty); }
			set { SetValue(PanelProperty, value); }
		}

		public bool IsSelected
		{
			get { return GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); PseudoClasses.Set(":active", value); }
		}

		public Tab(ToolWindow panel, TabGroup group)
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
}
