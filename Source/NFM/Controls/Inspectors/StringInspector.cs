using System;
using System.ComponentModel;
using System.Numerics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.LogicalTree;

namespace NFM;

[CustomInspector(typeof(string))]
public partial class StringInspector : UserControl
{
	public StringInspector()
	{
		DataContext = this;

		// Create TextInput and bind to inspected property.
		Content = new TextInput()
		{
			[!NFM.TextInput.ValueProperty] = new Binding(nameof(Value))
		};
	}

	public void Dispose()
	{

	}
}
