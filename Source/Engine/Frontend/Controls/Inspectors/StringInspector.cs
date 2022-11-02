using System;
using System.ComponentModel;
using System.Numerics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;

namespace Engine.Frontend
{
	[CustomInspector(typeof(string))]
	public partial class StringInspector : UserControl
	{
		public StringInspector()
		{
			// Create TextInput and bind to inspected property.
			var textInput = new TextInput();
			textInput.Bind(Frontend.TextInput.ValueProperty, nameof(Value));

			// Set inspector content.
			Content = textInput;
		}

		public void Dispose()
		{

		}
	}
}
