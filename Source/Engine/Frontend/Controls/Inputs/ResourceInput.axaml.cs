using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Engine.Resources;

namespace Engine.Frontend
{
	public class ResourceInput : TemplatedControl
	{
		public static StyledProperty<Resource> ValueProperty = AvaloniaProperty.Register<ResourceInput, Resource>(nameof(Value));

		[Notify] public Resource Value
		{
			get => GetValue(ValueProperty);
			set => SetValue(ValueProperty, value);
		}
	}

	public class ResourceValueConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
			{
				return "--";
			}
			else if (value is Resource resource)
			{
				if (resource.Source != null)
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
}
