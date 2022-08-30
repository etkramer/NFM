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
	public class ResourceInput : BaseInput
	{
		private string Value
		{
			get
			{
				Resource res = GetFirstValue<Resource>();
				return HasMultipleValues ? "--" : res.Source == null ? res.GetType().Name : $"{res.Source.Path.Split('/').Last()} ({res.GetType().Name})";
			}
		}

		public ResourceInput(PropertyInfo property) : base(property)
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

			TextBlock textDisplay = new TextBlock();
			textDisplay.Padding = new(4, 0);
			textDisplay.VerticalAlignment = VerticalAlignment.Center;
			textDisplay.Bind(TextBlock.TextProperty, nameof(Value), this);
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