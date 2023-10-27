using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using NFM.Generators;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace NFM;

[DeclarativeProperty(SetterName = "Focusable", PropertyName = "Focusable", BindingPropertyName = "FocusableProperty", PropertyType = typeof(bool), TargetType = typeof(InputElement))]

// Control
[DeclarativeProperty(SetterName = "VerticalAlignment", PropertyName = "VerticalAlignment", BindingPropertyName = "VerticalAlignmentProperty", PropertyType = typeof(VerticalAlignment), TargetType = typeof(Control))]
[DeclarativeProperty(SetterName = "HorizontalAlignment", PropertyName = "HorizontalAlignment", BindingPropertyName = "HorizontalAlignmentProperty", PropertyType = typeof(HorizontalAlignment), TargetType = typeof(Control))]
[DeclarativeProperty(SetterName = "Width", PropertyName = "Width", BindingPropertyName = "WidthProperty", PropertyType = typeof(double), TargetType = typeof(Control))]
[DeclarativeProperty(SetterName = "Height", PropertyName = "Height", BindingPropertyName = "HeightProperty", PropertyType = typeof(double), TargetType = typeof(Control))]

// TemplatedControl
[DeclarativeProperty(SetterName = "Background", PropertyName = "Background", BindingPropertyName = "BackgroundProperty", PropertyType = typeof(Brush), TargetType = typeof(TemplatedControl))]

// ContentControl
[DeclarativeProperty(SetterName = "Content", PropertyName = "Content", BindingPropertyName = "ContentProperty", PropertyType = typeof(object), TargetType = typeof(ContentControl))]

public static partial class ControlExt
{
	public static void Bind(this Control subject, AvaloniaProperty property, string propertyName, object source = null, BindingMode mode = BindingMode.Default)
	{
		AvaloniaObjectExtensions.Bind(subject, property, new Binding(propertyName, mode), source);
	}

	public static Brush GetResourceBrush(this Control subject, string resourceName)
	{
		object resource = Application.Current.FindResource(resourceName);
		if (resource is Avalonia.Media.Color color)
		{
			return new SolidColorBrush(color);
		}
		else if (resource is Brush)
		{
			return (Brush)resource;
		}

		return null;
	}

	public static Bitmap GetResourceBitmap(this Control subject, string uri)
	{
            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            var asset = assets.Open(new Uri(uri));

            return new Bitmap(asset);
	}

	public static T With<T>(this T subject, Action<T> action) where T : Control
	{
		action?.Invoke(subject);
		return subject;
	}

	public static T Style<T>(this T subject, params string[] styleClasses) where T : Control
	{
		subject.Classes = new Classes(styleClasses);
		return subject;
	}
}
