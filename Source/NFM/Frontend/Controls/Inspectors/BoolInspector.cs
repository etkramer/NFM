using System;
using System.ComponentModel;
using System.Numerics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.LogicalTree;

namespace NFM.Frontend
{
	[CustomInspector(typeof(bool))]
	public partial class BoolInspector : UserControl
	{
		public BoolInspector()
		{
			// Create BoolInput and bind to inspected property.
			Content = new BoolInput()
			{
				[!BoolInput.ValueProperty] = new Binding(nameof(Value))
			};
		}

		public void Dispose()
		{

		}
	}
}
