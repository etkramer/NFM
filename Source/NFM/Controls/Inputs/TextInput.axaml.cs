using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using ReactiveUI;

namespace NFM;

public class TextInput : TemplatedControl, IActivatableView
{
	public static StyledProperty<string> ValueProperty = AvaloniaProperty.Register<TextInput, string>(nameof(Value), defaultBindingMode: BindingMode.TwoWay);

	[Notify] public string Value
	{
		get => GetValue(ValueProperty);
		set
		{
			SetValue(ValueProperty, value);
		}
	}

	[Notify]
	string valueProxy { get; set; }

	public TextInput()
	{
		this.WhenActivated(d =>
		{
			// Make sure proxy responds to changes in source.
			valueProxy = Value;
			ValueProperty.Changed
				.Where(o => o.Sender == this)
				.Subscribe(o => valueProxy = Value)
				.DisposeWith(d);
		});
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		var textBox = e.NameScope.Find<TextBox>("PART_TextBox");
		textBox.KeyDown += OnKeyDown;
		textBox.LostFocus += OnLostFocus;

		base.OnApplyTemplate(e);
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
		// Apply value.
		Value = valueProxy;
	}
}
