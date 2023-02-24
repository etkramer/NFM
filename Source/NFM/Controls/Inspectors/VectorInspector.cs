using System;
using System.ComponentModel;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.LogicalTree;
using Avalonia.Media;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace NFM;

[CustomInspector(typeof(Vector2), typeof(Vector2i), typeof(Vector3), typeof(Vector3i), typeof(Vector4), typeof(Vector4i))]
public partial class VectorInspector : UserControl
{
	[Reactive]
	private object ValueX
	{
		set
		{
			var vector = Value;
			vecIndexer.SetValue(vector, value, new object[] {0});
			Value = vector;
		}
		get
		{
			return vecIndexer.GetValue(Value, new object[] {0});
		}
	}

	[Reactive]
	private object ValueY
	{
		set
		{
			var vector = Value;
			vecIndexer.SetValue(vector, value, new object[] {1});
			Value = vector;
		}
		get
		{
			return vecIndexer.GetValue(Value, new object[] {1});
		}
	}

	[Reactive]
	private object ValueZ
	{
		set
		{
			var vector = Value;
			vecIndexer.SetValue(vector, value, new object[] {2});
			Value = vector;
		}
		get
		{
			return vecIndexer.GetValue(Value, new object[] {2});
		}
	}

	[Reactive]
	private object ValueW
	{
		set
		{
			var vector = Value;
			vecIndexer.SetValue(vector, value, new object[] {3});
			Value = vector;
		}
		get
		{
			return vecIndexer.GetValue(Value, new object[] {3});
		}
	}

	private PropertyInfo vecIndexer;

	public VectorInspector()
	{
		DataContext = this;

		this.WhenAnyValue(o => o.Value)
			.Subscribe(o =>
			{
				RaisePropertyChanged(nameof(ValueX));
				RaisePropertyChanged(nameof(ValueY));
				RaisePropertyChanged(nameof(ValueZ));
				RaisePropertyChanged(nameof(ValueW));
			});

		// Count components and grab this[] indexer property.
		int numComponents = GetNumComponents(Property.PropertyType);
		vecIndexer = Property.PropertyType.GetProperty("Item");

		// Loop through vector components...
		List<Control> inputs = new();
		for (int i = 0; i < numComponents; i++)
		{
			// Create component input.
			NumInput input = new NumInput();
			input.With(o => o.Icon = GetIconChar(i));
			input.With(o => o.IconColor = GetIconForeground(i));
			input.Margin(new Thickness(i == 0 ? 0 : 4, 0, (i == numComponents - 1 ? 0 : 4), 0));
			input.Bind(NumInput.ValueProperty, i switch
			{
				0 => nameof(ValueX),
				1 => nameof(ValueY),
				2 => nameof(ValueZ),
				3 => nameof(ValueW),
				_ => null
			}, this);
			
			inputs.Add(input);
		}

		Content = new UniformGrid()
			.With(o => o.Columns = numComponents)
			.Children(inputs.ToArray());
	}

	private int GetNumComponents(Type type)
	{
		if (type == typeof(Vector2) || type == typeof(Vector2i))
		{
			return 2;
		}
		else if (type == typeof(Vector3) || type == typeof(Vector3i))
		{
			return 3;
		}
		else if (type == typeof(Vector4) || type == typeof(Vector4i))
		{
			return 4;
		}

		return 0;
	}

	private string GetIconChar(int component) => component switch
	{
		0 => "X",
		1 => "Y",
		2 => "Z",
		3 => "W",
		_ => null
	};

	private SolidColorBrush GetIconForeground(int component) => foregroundBrushes[component];
	private static SolidColorBrush[] foregroundBrushes = new[]
	{
		SolidColorBrush.Parse("#E26E6E"),
		SolidColorBrush.Parse("#A8CC60"),
		SolidColorBrush.Parse("#84B5E6"),
		SolidColorBrush.Parse("#6E6EE2"),
	};
}
