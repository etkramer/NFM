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
		public static readonly StyledProperty<ToolPanel> PanelProperty = AvaloniaProperty.Register<Tab, ToolPanel>(nameof(Panel));
		public static readonly StyledProperty<bool> IsSelectedProperty = AvaloniaProperty.Register<Tab, bool>(nameof(IsSelected), false);

		private TabGroup group;

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

		public Tab(ToolPanel panel, TabGroup group)
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
