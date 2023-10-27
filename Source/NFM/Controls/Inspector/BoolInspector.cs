using Avalonia.Controls;
using Avalonia.Data;

namespace NFM;

[CustomInspector(typeof(bool))]
public partial class BoolInspector : UserControl
{
	public BoolInspector()
	{
		DataContext = this;

		// Create BoolInput and bind to inspected property.
		Content = new BoolInput()
		{
			[!BoolInput.ValueProperty] = new Binding(nameof(Value))
		};
	}
}
