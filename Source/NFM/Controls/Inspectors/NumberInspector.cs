using System;
using System.ComponentModel;
using System.Numerics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.LogicalTree;

namespace NFM.Frontend
{
	[CustomInspector(typeof(INumber<>))]
	public partial class NumberInspector : UserControl
	{
		public NumberInspector()
		{
			// Create NumInput and bind to inspected property.
			Content = new NumInput()
			{
				[!NumInput.ValueProperty] = new Binding(nameof(Value))
			};
		}

		public void Dispose()
		{

		}
	}
}
