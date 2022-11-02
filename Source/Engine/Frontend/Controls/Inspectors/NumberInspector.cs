using System;
using System.ComponentModel;
using System.Numerics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;

namespace Engine.Frontend
{
	[CustomInspector(typeof(INumber<>))]
	public partial class NumberInspector : UserControl
	{
		public NumberInspector()
		{
			// Create NumInput and bind to inspected property.
			var numInput = new NumInput();
			numInput.Bind(NumInput.ValueProperty, nameof(Value));

			// Set inspector content.
			Content = numInput;
		}

		public void Dispose()
		{

		}
	}
}
