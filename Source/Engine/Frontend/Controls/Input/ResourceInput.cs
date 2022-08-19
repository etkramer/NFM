using System;
using System.ComponentModel;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Engine.Resources;

namespace Engine.Frontend
{
	public class ResourceInput : UserControl
	{
		public ResourceInput(Func<object> getter, Action<object> setter, bool hasMultipleValues)
		{
			Panel icon = new Panel()
				.Background("#19E6E62E")
				.Width(16)
				.Height(16)
				.Children(new TextBlock()
					.Text("\uE3DC")
					.Size(13)
					.Font(this.GetResource<FontFamily>("IconsFont"))
					.Foreground("#E6E62E")
					.VerticalAlignment(VerticalAlignment.Center)
					.HorizontalAlignment(HorizontalAlignment.Center)
				);

			Resource value = getter.Invoke() as Resource;
			string displayName = value.Source == null ? value.GetType().Name : $"{value.Source.Path.Split('/').Last()} ({value.GetType().Name})";

			TextBlock textDisplay = new TextBlock();
			textDisplay.Padding = new(4, 0);
			textDisplay.VerticalAlignment = VerticalAlignment.Center;
			textDisplay.Text = hasMultipleValues ? "--" : displayName;
			textDisplay.Foreground = this.GetResourceBrush("ThemeForegroundMidBrush");
			textDisplay.TextTrimming = TextTrimming.CharacterEllipsis;

			Content = new ContentControl()
				.Radius(2)
				.Background(this.GetResourceBrush("ControlBackground"))
				.With(o => o.Padding = new(1))
				.Content(
					new Grid()
						.Columns("auto, *")
						.Children(icon.Column(0), textDisplay.Column(1))
				);
		}
	}
}