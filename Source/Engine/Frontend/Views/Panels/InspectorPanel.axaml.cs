using System;
using System.ComponentModel;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using ReactiveUI;

namespace Engine.Frontend
{
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
}