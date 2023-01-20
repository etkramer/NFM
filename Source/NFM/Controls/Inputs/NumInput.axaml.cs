using System.Data;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Media;

namespace NFM;

public class NumInput : TemplatedControl
{
	public static StyledProperty<object> ValueProperty = AvaloniaProperty.Register<NumInput, object>(nameof(Value), defaultBindingMode: BindingMode.TwoWay);

	public static StyledProperty<string> IconProperty = AvaloniaProperty.Register<NumInput, string>(nameof(Icon));
	public static StyledProperty<IBrush> IconColorProperty = AvaloniaProperty.Register<NumInput, IBrush>(nameof(IconColor));

	[Notify] public string Icon
	{
		get => GetValue(IconProperty);
		set
		{
			SetValue(IconProperty, value);
		}
	}

	[Notify] public object Value
	{
		get => GetValue(ValueProperty);
		set
		{
			SetValue(ValueProperty, value);
		}
	}

	[Notify] public IBrush IconColor
	{
		get => GetValue(IconColorProperty);
		set
		{
			SetValue(IconColorProperty, value);
		}
	}

	// Stores string changes before they've been applied.
	private string value;
	[Notify] private string valueProxy
	{
		get => Value?.ToString();
		set => this.value = value;
	}

	private TextBox textBox;
	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		textBox = e.NameScope.Find<TextBox>("PART_TextBox");
		textBox.KeyDown += OnKeyDown;
		textBox.LostFocus += OnLostFocus;
		textBox.IsUndoEnabled = false;

		// Ignore alphabetical inputs.
		textBox.AddHandler(TextInputEvent, (o, e) =>
		{
			if (!e.Text.All(c => !char.IsLetter(c)))
			{
				e.Handled = true;
			}
		},
		RoutingStrategies.Tunnel);

		// Make sure proxy responds to changes in source.
		ValueProperty.Changed.Subscribe(o => (this as INotify).Raise(nameof(valueProxy)));

		base.OnApplyTemplate(e);
	}

	private void OnKeyDown(object sender, KeyEventArgs args)
	{
		if (args.Key == Key.Enter)
		{
			Focus();
		}
	}

	private void OnLostFocus(object sender, RoutedEventArgs args)
	{
		if (Value == null)
		{
			return;
		}

		// Set input to new value.
		if (TryParseNum(value ?? Value.ToString(), Value.GetType(), out object num))
		{
			Value = num;

			// Make sure text field resets even if the number hasn't changed.
			(this as INotify).Raise(nameof(valueProxy));
		}
		else
		{
			// Reset value.
			valueProxy = Value.ToString();
			textBox.Text = value;
		}
	}

	private static DataTable computeTable = new();
	private bool TryParseNum(string text, Type numType, out object num)
	{
		// Interpret emptied field as zero.
		if (string.IsNullOrWhiteSpace(text))
		{
			num = Convert.ChangeType(0, numType);
			return true;
		}

		// Try to simplify the text as an expression.
		try
		{
			object computedValue = computeTable.Compute(text, null);
			text = computedValue.ToString();
		}
		catch {} // No problem, probably just not a valid expression.

		if (IsFloat(numType))
		{
			if (double.TryParse(text, out double floatValue))
			{
				num = Convert.ChangeType(floatValue, numType);
				return true;
			}
		}
		else
		{
			if (IsUnsigned(numType) && ulong.TryParse(text, out ulong uintValue))
			{
				num = Convert.ChangeType(uintValue, numType);
				return true;
			}
			else if (!IsUnsigned(numType) && long.TryParse(text, out long intValue))
			{
				num = Convert.ChangeType(intValue, numType);
				return true;
			}
		}

		num = null;
		return false;
	}

	private bool IsUnsigned(Type type)
	{
		return type == typeof(byte)
			|| type == typeof(ushort)
			|| type == typeof(uint)
			|| type == typeof(ulong);
	}

	private bool IsFloat(Type type)
	{
		return type == typeof(float)
			|| type == typeof(double);
	}
}
