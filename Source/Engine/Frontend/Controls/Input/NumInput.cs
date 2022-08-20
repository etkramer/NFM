using System;
using System.ComponentModel;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;

namespace Engine.Frontend
{
	public class NumInput : UserControl
	{
		public NumInput(IEnumerable<object> subjects, PropertyInfo property)
			: this(() => property.GetValue(subjects.First()), (o) => subjects.ForEach(subject => property.SetValue(subject, Convert.ChangeType(o, property.PropertyType))), subjects.HasVariation((o) => property.GetValue(o)))
		{

		}

		public NumInput(Func<object> getter, Action<object> setter, bool hasMultipleValues, char iconOverride = '\uE3C9')
		{
			Control icon = new Panel()
				.Background("#19E6E62E")
				.Width(16)
				.Height(16)
				.Children(new TextBlock()
					.Text(iconOverride.ToString())
					.Size(12)
					.Font(this.GetResource<FontFamily>("IconsFont"))
					.Foreground("#E6E62E")
					.VerticalAlignment(VerticalAlignment.Center)
					.HorizontalAlignment(HorizontalAlignment.Center)
				);

			TextBox numEntry = new TextBox();
			numEntry.Padding = new(4, 0);
			numEntry.VerticalContentAlignment = VerticalAlignment.Center;
			numEntry.Text = hasMultipleValues ? "--" : getter.Invoke().ToString();
			numEntry.Foreground = this.GetResourceBrush("ThemeForegroundMidBrush");

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
					// Integer values
					if (long.TryParse(numEntry.Text, out long intResult))
					{
						// Apply value to subjects.
						setter.Invoke(intResult);
					}
					// Floating point values
					else if (double.TryParse(numEntry.Text, out double floatResult))
					{
						// Apply value to subjects.
						setter.Invoke(floatResult);
					}
					// Invalid value
					else
					{
						// Reset input.
						numEntry.Text = hasMultipleValues ? "--" : getter.Invoke().ToString();
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
						.Children(icon.Column(0), numEntry.Column(1))
				);
		}

		private bool IsKeyNumeric(Key key)
		{
			return key == Key.D0
				|| key == Key.D1
				|| key == Key.D2
				|| key == Key.D3
				|| key == Key.D4
				|| key == Key.D5
				|| key == Key.D6
				|| key == Key.D7
				|| key == Key.D8
				|| key == Key.D9
				|| key == Key.OemMinus
				|| key == Key.OemPeriod
				|| key == Key.Back
				|| key == Key.Enter;
		}
	}
}