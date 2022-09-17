using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Engine.Resources;

namespace Engine.Frontend
{
	public class ResourceInput : TemplatedControl
	{
		public static StyledProperty<Resource> ValueProperty = AvaloniaProperty.Register<ResourceInput, Resource>(nameof(Value), defaultBindingMode: BindingMode.TwoWay);

		public static AvaloniaProperty<string> ValueTextProperty = AvaloniaProperty.RegisterDirect<ResourceInput, string>(nameof(ValueText), o => o.ValueText);

		[Notify] public Resource Value
		{
			get => GetValue(ValueProperty);
			set
			{
				SetValue(ValueProperty, value);

			}
		}

		private string ValueText
		{
			get
			{
				if (Value == null)
				{
					return "--";
				}
				else if (Value.Source != null)
				{
					return $"{Value.Source.Path.Split('/').Last()} ({Value.GetType().Name})";
				}
				else
				{
					return Value.GetType().Name;
				}
			}
		}
	}
}
