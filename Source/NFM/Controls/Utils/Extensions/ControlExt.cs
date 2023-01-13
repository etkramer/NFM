using System;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using System.Runtime.CompilerServices;
using System.Reflection;
using Avalonia.Controls.Primitives;
using System.Linq;
using System.Collections;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Input;
using NFM.Aspects;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace NFM.Frontend
{
	[DeclarativeProperty(SetterName = "Focusable", PropertyName = "Focusable", BindingPropertyName = "FocusableProperty", PropertyType = typeof(bool), TargetType = typeof(InputElement))]
	[DeclarativeProperty(SetterName = "Header", PropertyName = "Header", PropertyType = typeof(string), TargetType = typeof(HeaderedContentControl))]

	// Control
	[DeclarativeProperty(SetterName = "VerticalAlignment", PropertyName = "VerticalAlignment", BindingPropertyName = "VerticalAlignmentProperty", PropertyType = typeof(VerticalAlignment), TargetType = typeof(Control))]
	[DeclarativeProperty(SetterName = "HorizontalAlignment", PropertyName = "HorizontalAlignment", BindingPropertyName = "HorizontalAlignmentProperty", PropertyType = typeof(HorizontalAlignment), TargetType = typeof(Control))]
	[DeclarativeProperty(SetterName = "Width", PropertyName = "Width", BindingPropertyName = "WidthProperty", PropertyType = typeof(double), TargetType = typeof(Control))]
	[DeclarativeProperty(SetterName = "Height", PropertyName = "Height", BindingPropertyName = "HeightProperty", PropertyType = typeof(double), TargetType = typeof(Control))]
	[DeclarativeProperty(SetterName = "IsVisible", PropertyName = "IsVisible", BindingPropertyName = "IsVisibleProperty", PropertyType = typeof(bool), TargetType = typeof(Control))]

	// TemplatedControl
	[DeclarativeProperty(SetterName = "Foreground", PropertyName = "Foreground", BindingPropertyName = "ForegroundProperty", PropertyType = typeof(Brush), TargetType = typeof(TemplatedControl))]
	[DeclarativeProperty(SetterName = "Background", PropertyName = "Background", BindingPropertyName = "BackgroundProperty", PropertyType = typeof(Brush), TargetType = typeof(TemplatedControl))]
	[DeclarativeProperty(SetterName = "BorderWidth", PropertyName = "BorderThickness", BindingPropertyName = "BorderThicknessProperty", PropertyType = typeof(Thickness), TargetType = typeof(TemplatedControl))]

	[DeclarativeProperty(SetterName = "Items", PropertyName = "Items", BindingPropertyName = "ItemsProperty", PropertyType = typeof(IEnumerable), TargetType = typeof(ItemsControl))]
	[DeclarativeProperty(SetterName = "ItemTemplate", PropertyName = "ItemTemplate", BindingPropertyName = "ItemTemplateProperty", PropertyType = typeof(IDataTemplate), TargetType = typeof(ItemsControl))]

	// ContentControl
	[DeclarativeProperty(SetterName = "Content", PropertyName = "Content", BindingPropertyName = "ContentProperty", PropertyType = typeof(object), TargetType = typeof(ContentControl))]
	[DeclarativeProperty(SetterName = "Border", PropertyName = "BorderBrush", BindingPropertyName = "BorderBrushProperty", PropertyType = typeof(Brush), TargetType = typeof(ContentControl))]
	[DeclarativeProperty(SetterName = "Radius", PropertyName = "CornerRadius", BindingPropertyName = "CornerRadiusProperty", PropertyType = typeof(CornerRadius), TargetType = typeof(TemplatedControl))]

	[DeclarativeProperty(SetterName = "SelectionMode", PropertyName = "SelectionMode", PropertyType = typeof(SelectionMode), TargetType = typeof(TreeView))]
	[DeclarativeProperty(SetterName = "IsExpanded", PropertyName = "IsExpanded", PropertyType = typeof(bool), TargetType = typeof(Expander))]

	public static partial class ControlExt
	{
		public static void Bind(this IControl subject, AvaloniaProperty property, string propertyName, object source = null, BindingMode mode = BindingMode.Default)
		{
			AvaloniaObjectExtensions.Bind(subject, property, new Binding(propertyName, mode), source);
		}

		public static Binding BindTo(this IDataTemplate subject, string propertyName, object sourceObject = null) => BindTo(subject as IControl, propertyName, sourceObject);
		public static Binding BindTo(this IControl subject, string propertyName, object sourceObject = null)
		{
			Binding binding = new Binding(propertyName);
			if (sourceObject != null)
				binding.Source = sourceObject;

			return binding;
		}

		public static T ItemsSource<T>(this T subject, string binding) where T : TreeDataTemplate
		{
			subject.ItemsSource = BindTo(subject, binding);
			return subject;
		}

		public static T Content<T>(this T subject, IControl content) where T : TreeDataTemplate
		{
			subject.Content = content;
			return subject;
		}

		public static T DataType<T>(this T subject, Type type) where T : TreeDataTemplate
		{
			subject.DataType = type;
			return subject;
		}

		public static T GetResource<T>(this IControl subject, string resourceName)
		{
			T res = (T)Application.Current.FindResource(resourceName);
			return res;
		}

		public static Brush GetResourceBrush(this IControl subject, string resourceName)
		{
			object resource = Application.Current.FindResource(resourceName);
			if (resource is Avalonia.Media.Color)
			{
				return new SolidColorBrush((Avalonia.Media.Color)resource);
			}
			else if (resource is Brush)
			{
				return (Brush)resource;
			}

			return null;
		}

		public static Bitmap GetResourceBitmap(this IControl subject, string uri)
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

		public static T OnClick<T>(this T subject, Action command) where T : MenuItem
		{
			subject.Command = new ButtonExt.CommandImpl(command);
			return subject;
		}

		public static T Tooltip<T>(this T subject, string tooltip) where T : Control
		{
			ToolTip.SetTip(subject, tooltip);
			return subject;
		}

		public static T ContextMenu<T>(this T subject, params Control[] menuContent) where T : Control
		{
			subject.ContextMenu = new ContextMenu().Items(menuContent as IEnumerable);
			return subject;
		}
	}
}
