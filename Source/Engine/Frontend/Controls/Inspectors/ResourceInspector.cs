using System;
using System.ComponentModel;
using System.Numerics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Engine.Resources;

namespace Engine.Frontend
{
	[CustomInspector(typeof(Resource))]
	public partial class ResourceInspector : UserControl
	{
		public ResourceInspector()
		{
			// Create BoolInput and bind to inspected property.
			var resourceInput = new ResourceInput();
			resourceInput.Bind(ResourceInput.ValueProperty, nameof(Value));

			// Set inspector content.
			Content = resourceInput;
		}

		public void Dispose()
		{

		}
	}
}