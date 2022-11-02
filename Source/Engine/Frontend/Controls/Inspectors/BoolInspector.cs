using System;
using System.ComponentModel;
using System.Numerics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;

namespace Engine.Frontend
{
	[CustomInspector(typeof(bool))]
	public partial class BoolInspector : UserControl
	{
		public BoolInspector()
		{
			// Create BoolInput and bind to inspected property.
			var boolInput = new BoolInput();
			boolInput.Bind(BoolInput.ValueProperty, nameof(Value));

			// Set inspector content.
			Content = boolInput;
		}

		public void Dispose()
		{

		}
	}
}
