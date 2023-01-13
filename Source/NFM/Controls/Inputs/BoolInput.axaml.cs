using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;

namespace NFM.Frontend
{
	public class BoolInput : TemplatedControl
	{
		public static StyledProperty<bool> ValueProperty = AvaloniaProperty.Register<BoolInput, bool>(nameof(Value), defaultBindingMode: BindingMode.TwoWay);

		[Notify] public bool Value
		{
			get => GetValue(ValueProperty);
			set
			{
				SetValue(ValueProperty, value);
			}
		}
	}
}
