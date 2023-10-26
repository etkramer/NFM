using Avalonia.Controls;
using Avalonia.Data;

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
}
