using System;
using System.ComponentModel;
using System.Numerics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.LogicalTree;
using Engine.Resources;

namespace Engine.Frontend
{
	[CustomInspector(typeof(Resource))]
	public partial class ResourceInspector : UserControl
	{
		public ResourceInspector()
		{
			// Create ResourceInput and bind to inspected property.
			Content = new ResourceInput()
			{
				[!ResourceInput.ValueProperty] = new Binding(nameof(Value))
			};
		}

		public void Dispose()
		{

		}
	}
}