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
		public StringInput(IEnumerable<object> subjects, PropertyInfo property)
		{
			bool hasMultipleValues = subjects.HasVariation((o) => property.GetValue(o));

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
			textEntry.Text = hasMultipleValues ? "--" : property.GetValue(subjects.First()) as string;
			textEntry.Foreground = this.GetResourceBrush("ThemeForegroundMidBrush");

			// Ignore non-numeric inputs.
			textEntry.KeyDown += (o, e) =>
			{
				// Hit enter?
				if (e.Key == Key.Enter)
				{
					// Apply value to subjects.
					foreach (object subject in subjects)
					{
						property.SetValue(subject, Convert.ChangeType(textEntry.Text, property.PropertyType));
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