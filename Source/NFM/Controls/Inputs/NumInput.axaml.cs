using System.Data;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Media;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace NFM;

public class NumInput : TemplatedControl, IActivatableView
{
	public static StyledProperty<object> ValueProperty = AvaloniaProperty.Register<NumInput, object>(nameof(Value), defaultBindingMode: BindingMode.TwoWay);

	public static StyledProperty<string> IconProperty = AvaloniaProperty.Register<NumInput, string>(nameof(Icon));
	public static StyledProperty<IBrush> IconColorProperty = AvaloniaProperty.Register<NumInput, IBrush>(nameof(IconColor));

	[Reactive]
	public object Value
	{
		get => GetValue(ValueProperty);
		set
		{
			SetValue(ValueProperty, value);
		}
	}

	[Reactive]
	public string Icon
	{
		get => GetValue(IconProperty);
		set
		{
			SetValue(IconProperty, value);
		}
	}

	[Reactive]
	public IBrush IconColor
	{
		get => GetValue(IconColorProperty);
		set
		{
			SetValue(IconColorProperty, value);
		}
	}

	[Notify]
	string valueProxy { get; set; }

	public NumInput()
	{
		this.WhenActivated(d =>
		{
			// Make sure proxy responds to changes in source
			valueProxy = FormatNumber(Value);
			ValueProperty.Changed
				.Where(o => o.Sender == this)
				.Subscribe(o => valueProxy = FormatNumber(Value))
				.DisposeWith(d);
		});
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		var textBox = e.NameScope.Find<TextBox>("PART_TextBox");
		textBox.KeyDown += OnKeyDown;
		textBox.LostFocus += OnLostFocus;
		textBox.AddHandler(TextInputEvent, (o, e) =>
		{
			e.Handled = !e.Text.All(c => !char.IsLetter(c));
		},
		RoutingStrategies.Tunnel);

		base.OnApplyTemplate(e);
	}

	string FormatNumber(object value)
	{
		return string.Format("{0:0.##}", value);
	}

	void OnKeyDown(object sender, KeyEventArgs args)
	{
		if (args.Key == Key.Enter)
		{
			Focus();
		}
	}

	void OnLostFocus(object sender, RoutedEventArgs args)
	{
		if (Value == null)
		{
			return;
		}

		// Set input to new value.
		if (TryParseNum(valueProxy, Value.GetType(), out object num))
		{
			Value = num;
		}

		// Make sure text field resets even if the number hasn't changed
		valueProxy = Value.ToString();
	}

	static DataTable computeTable = new();
	bool TryParseNum(string text, Type numType, out object num)
	{
		// Interpret emptied field as zero
		if (string.IsNullOrWhiteSpace(text))
		{
			num = Convert.ChangeType(0, numType);
			return true;
		}

		// Try to simplify the text as an expression
		try
		{
			object computedValue = computeTable.Compute(text, null);
			text = computedValue.ToString();
		}
		catch {} // No problem, probably just not a valid expression

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

	bool IsUnsigned(Type type)
	{
		return type == typeof(byte)
			|| type == typeof(ushort)
			|| type == typeof(uint)
			|| type == typeof(ulong);
	}

	bool IsFloat(Type type)
	{
		return type == typeof(float)
			|| type == typeof(double);
	}
}
