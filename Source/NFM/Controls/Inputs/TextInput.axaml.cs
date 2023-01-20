using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;

namespace NFM;

public class TextInput : TemplatedControl
{
	public static StyledProperty<string> ValueProperty = AvaloniaProperty.Register<TextInput, string>(nameof(Value), defaultBindingMode: BindingMode.TwoWay);
	public static AvaloniaProperty<string> ValueProxyProperty = AvaloniaProperty.RegisterDirect<TextInput, string>(nameof(ValueProxy), o => o.ValueProxy, (o, v) => o.ValueProxy = v);

	[Notify] public string Value
	{
		get => GetValue(ValueProperty);
		set
		{
			SetValue(ValueProperty, value);
		}
	}

	// Stores string changes before they've been applied.
	private string value;
	private string ValueProxy
	{
		get => Value;
		set => this.value = value;
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		var textBox = e.NameScope.Find<TextBox>("PART_TextBox");
		textBox.KeyDown += OnKeyDown;
		textBox.LostFocus += OnLostFocus;
		textBox.IsUndoEnabled = false;

		// Make sure proxy responds to changes in source.
		ValueProperty.Changed.Subscribe(o => RaisePropertyChanged(ValueProxyProperty, default, Value));

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
		// Apply value.
		Value = value;
	}
}
