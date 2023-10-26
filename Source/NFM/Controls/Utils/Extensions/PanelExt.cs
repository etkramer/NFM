using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using NFM.Generators;

namespace NFM;

[DeclarativeProperty(SetterName = "Background", PropertyName = "Background", BindingPropertyName = "BackgroundProperty", PropertyType = typeof(Brush), TargetType = typeof(Panel))]
[DeclarativeProperty(SetterName = "Columns", PropertyName = "ColumnDefinitions", PropertyType = typeof(ColumnDefinitions), TargetType = typeof(Grid))]
[DeclarativeProperty(SetterName = "Rows", PropertyName = "RowDefinitions", PropertyType = typeof(RowDefinitions), TargetType = typeof(Grid))]
[DeclarativeProperty(SetterName = "Margin", PropertyName = "Margin", PropertyType = typeof(Thickness), TargetType = typeof(Layoutable))]
[DeclarativeProperty(SetterName = "Spacing", PropertyName = "Spacing", PropertyType = typeof(double), TargetType = typeof(StackPanel))]
[DeclarativeProperty(SetterName = "Orientation", PropertyName = "Orientation", PropertyType = typeof(Orientation), TargetType = typeof(StackPanel))]
[DeclarativeProperty(SetterName = "Header", PropertyName = "Header", PropertyType = typeof(string), TargetType = typeof(HeaderedSelectingItemsControl))]
public static partial class PanelExt
{
	public static T Children<T>(this T subject, params Control[] items) where T : Panel => Children(subject, items as IEnumerable<Control>);
	public static T Children<T>(this T subject, IEnumerable<Control> items) where T : Panel
	{
		if (items != null)
		{
			subject.Children.Clear();
			subject.Children.AddRange(items);
		}

		return subject;
	}

	public static T Column<T>(this T subject, int column) where T : Control
	{
		subject.SetValue(Grid.ColumnProperty, column);
		return subject;
	}

	public static T Row<T>(this T subject, int row) where T : Control
	{
		subject.SetValue(Grid.RowProperty, row);
		return subject;
	}
}
