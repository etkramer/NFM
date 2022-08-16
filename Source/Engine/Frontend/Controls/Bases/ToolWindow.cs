using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Engine.Frontend.Controls;

namespace Engine.Frontend
{
	public abstract class ToolWindow : UserControl
	{
		public static readonly StyledProperty<string> TitleProperty = AvaloniaProperty.Register<ToolWindow, string>(nameof(Title), "Tool Window");

		public string Title
		{
			get { return GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}

		public static T Spawn<T>(TabGroup group = null) where T : ToolWindow, new()
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
