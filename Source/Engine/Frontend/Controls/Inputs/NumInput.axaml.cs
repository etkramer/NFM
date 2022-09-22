using System.Data;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;

namespace Engine.Frontend
{
	public class NumInput : TemplatedControl
	{
		public static StyledProperty<string> IconProperty = AvaloniaProperty.Register<NumInput, string>(nameof(Icon), defaultValue: "\uE3C9");
		public static StyledProperty<object> ValueProperty = AvaloniaProperty.Register<NumInput, object>(nameof(Value), defaultBindingMode: BindingMode.TwoWay);
		public static AvaloniaProperty<string> ValueProxyProperty = AvaloniaProperty.RegisterDirect<NumInput, string>(nameof(ValueProxy), o => o.ValueProxy, (o, v) => o.ValueProxy = v);

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

		// Stores string changes before they've been applied.
		private string value;
		[Notify] private string ValueProxy
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
			ValueProperty.Changed.Subscribe(o => RaisePropertyChanged(ValueProxyProperty, default, Value.ToString()));

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
			// Set input to new value.
			if (TryParseNum(value, Value.GetType(), out object num))
			{
				Value = num;
			}
			else
			{
				// Reset value.
				SetValue(ValueProxyProperty, Value.ToString());
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
}
