using Avalonia.Controls;
using Avalonia.Data;
using NFM.Resources;

namespace NFM;

[CustomInspector(typeof(GameResource))]
public partial class ResourceInspector : UserControl
{
	public ResourceInspector()
	{
		DataContext = this;

		// Create ResourceInput and bind to inspected property.
		Content = new ResourceInput()
		{
			[!ResourceInput.ValueProperty] = new Binding(nameof(Value))
		};
	}
}