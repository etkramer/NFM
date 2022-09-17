using System;
using System.ComponentModel;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;

namespace Engine.Frontend
{
	public class StringInput : BaseInput
	{
		[Notify] private string Value
		{
			get
			{
				return HasMultipleValues ? "--" : GetFirstValue<string>();
			}
			set
			{
				if (value != "--")
				{
					SetValue(value);
				}
			}
		}

		public StringInput(PropertyInfo property, IEnumerable<object> subjects) : base(property, subjects)
		{
			OnSelectedPropertyChanged += () => (this as INotify).Raise(nameof(Value));

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
			textEntry.Bind(TextBox.TextProperty, nameof(Value), this);
			textEntry.Foreground = this.GetResourceBrush("ThemeForegroundMidBrush");
			textEntry.LostFocus += (o, e) => (this as INotify).Raise(nameof(Value));

			// Respond to keypresses.
			textEntry.KeyDown += (o, e) =>
			{
				// Hit enter?
				if (e.Key == Key.Enter)
				{
					// Switch focus to this instead.
					Focus();
				}
			};

			Content = new ContentControl()
				.Radius(2)
				.Background(this.GetResourceBrush("ControlBackgroundColor"))
				.With(o => o.Padding = new(1))
				.Content(
					new Grid()
						.Columns("auto, *")
						.Children(icon.Column(0), textEntry.Column(1))
				);
		}
	}
}