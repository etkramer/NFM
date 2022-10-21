using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Engine.Frontend
{
	public class Toolbar : TemplatedControl
	{
		public static readonly AvaloniaProperty OrientationProperty = AvaloniaProperty.Register<Toolbar, Avalonia.Layout.Orientation>(nameof(Orientation));
		public Avalonia.Layout.Orientation Orientation
		{
			get => (Avalonia.Layout.Orientation)GetValue(OrientationProperty);
			set => SetValue(OrientationProperty, value);
		}

		public static readonly AvaloniaProperty ItemsProperty = AvaloniaProperty.Register<Toolbar, AvaloniaList<object>>(nameof(Items));
		public AvaloniaList<object> Items
		{
			get => (AvaloniaList<object>)GetValue(ItemsProperty);
			set => SetValue(ItemsProperty, value);
		}

		public Toolbar()
		{
			Items = new AvaloniaList<object>();
		}
	}
}
