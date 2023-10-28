using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using NFM.Resources;

namespace NFM;

public class ResourceInput : TemplatedControl
{
	public static StyledProperty<GameResource> ValueProperty = AvaloniaProperty.Register<ResourceInput, GameResource>(nameof(Value), defaultBindingMode: BindingMode.TwoWay);

	[Notify] public GameResource Value
	{
		get => GetValue(ValueProperty);
		set => SetValue(ValueProperty, value);
	}
}

public class ResourceValueConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is null)
		{
			return null;
		}
		else if (value is GameResource resource)
		{
			if (resource.Source is not null)
			{
				return $"{resource.Source.Path.Split('/').Last()} ({resource.GetType().Name})";
			}
			else
			{
				return resource.GetType().Name;
			}
		}

		throw new InvalidCastException();
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}
