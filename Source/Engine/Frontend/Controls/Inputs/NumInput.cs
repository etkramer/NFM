using System;
using System.ComponentModel;
using System.Reflection;
using Avalonia.Data;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Engine.Editor;
using System.Collections.Specialized;
using Avalonia.Interactivity;

namespace Engine.Frontend
{
	public class NumInput : BaseInput
	{
		[Notify] private string Value
		{
			get
			{
				return HasMultipleValues ? "--" : GetFirstValue<object>().ToString();
			}
		}

		public NumInput(PropertyInfo property, IEnumerable<object> subjects) : base(property, subjects)
		{
			OnSelectedPropertyChanged += () => (this as INotify).Raise(nameof(Value));

			Control icon = new Panel()
				.Background("#19E6E62E")
				.Width(16)
				.Height(16)
				.Children(new TextBlock()
					.Text("\uE3C9")
					.Size(12)
					.Font(this.GetResource<FontFamily>("IconsFont"))
					.Foreground("#E6E62E")
					.VerticalAlignment(VerticalAlignment.Center)
					.HorizontalAlignment(HorizontalAlignment.Center)
				);

			// Create input box.
			TextBox numEntry = new TextBox();
			numEntry.Padding = new(4, 0);
			numEntry.VerticalContentAlignment = VerticalAlignment.Center;
			numEntry.Bind(TextBox.TextProperty, nameof(Value), this);
			numEntry.Foreground = this.GetResourceBrush("ThemeForegroundMidBrush");
			numEntry.LostFocus += (o, e) => (this as INotify).Raise(nameof(Value));

			// Ignore alphabetical inputs.
			numEntry.AddHandler(TextInputEvent, (o, e) =>
			{
				if (!e.Text.All(c => !char.IsLetter(c)))
				{
					e.Handled = true;
				}
			},
			RoutingStrategies.Tunnel);

			// Respond to enter key.
			numEntry.KeyDown += (o, e) =>
			{
				if (e.Key == Key.Enter)
				{
					// Set input to new value.
					if (TryParseNum(numEntry.Text, Property.PropertyType, out object num))
					{
						SetValue(num);
					}

					// Switch focus.
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
						.Children(icon.Column(0), numEntry.Column(1))
				);
		}
	}
}