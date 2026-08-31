using ReactiveUI.Reactive;

namespace NFM;

public partial class InspectorPanel : ReactiveToolPanel<InspectorModel>
{
	public InspectorPanel()
	{
		ViewModel = new InspectorModel();

		this.WhenActivated(d =>
		{

		});

		InitializeComponent();
	}
}