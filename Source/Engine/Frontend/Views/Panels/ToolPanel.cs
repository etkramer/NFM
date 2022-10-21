using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Engine.Frontend
{
	public abstract class ToolPanel : UserControl
	{
		public static readonly StyledProperty<string> TitleProperty = AvaloniaProperty.Register<ToolPanel, string>(nameof(Title), "Tool Window");

		public string Title
		{
			get { return GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}

		public static T Spawn<T>(TabGroup group = null) where T : ToolPanel, new()
		{
			T tool = new();
			tool.Focusable = true;

			/*if (group == null)
			{
				FloatingWindow window = new FloatingWindow(tool);
				window.Show();
			}
			else*/
			{
				Tab tab = new Tab(tool, group);
				group.Tabs.Add(tab);
			}

			return tool;
		}
	}
}
