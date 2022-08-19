using System;
using System.ComponentModel;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;

namespace Engine.Frontend
{
	public class StringInput : UserControl
	{
		public StringInput(Func<object> getter, Action<object> setter, bool hasMultipleValues)
		{
			Panel icon = new Panel()
				.Background("#19E6E62E")
				.Width(16)
				.Height(16)
				.Children(new TextBlock()
					.Text("\uE3C9")
					.Size(13)
					.Font(this.GetResource<FontFamily>("IconsFont"))
					.Foreground("#E6E62E")
					.VerticalAlignment(VerticalAlignment.Center)
					.HorizontalAlignment(HorizontalAlignment.Center)
				);

			TextBox textEntry = new TextBox();
			textEntry.Padding = new(4, 0);
			textEntry.VerticalContentAlignment = VerticalAlignment.Center;
			textEntry.Text = hasMultipleValues ? "--" : getter.Invoke() as string;
			textEntry.Foreground = this.GetResourceBrush("ThemeForegroundMidBrush");

			// Respond to keypresses.
			textEntry.KeyDown += (o, e) =>
			{
				// Hit enter?
				if (e.Key == Key.Enter)
				{
					// Apply value to subjects.
					if (textEntry.Text != "--")
					{
						setter.Invoke(textEntry.Text);
					}

					// Switch focus to this instead.
					Focus();
				}
			};

			Content = new ContentControl()
				.Radius(2)
				.Background(this.GetResourceBrush("ControlBackground"))
				.With(o => o.Padding = new(1))
				.Content(
					new Grid()
						.Columns("auto, *")
						.Children(icon.Column(0), textEntry.Column(1))
				);
		}
	}
}