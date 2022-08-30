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
			set
			{
				if (TryParseNum(value, Property.PropertyType, out object num))
				{
					SetValue(num);
				}
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

			// Ignore non-numeric inputs.
			numEntry.KeyDown += (o, e) =>
			{
				if (!IsKeyNumeric(e.Key))
				{
					e.Handled = true;
				}
				// Hit enter?
				else if (e.Key == Key.Enter)
				{
					// Switch focus and reset input.
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
						.Children(icon.Column(0), numEntry.Column(1))
				);
		}
	}
}