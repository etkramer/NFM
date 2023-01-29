using System;
using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using ReactiveUI;

namespace NFM;

public partial class OutlinerPanel : ReactiveToolPanel<OutlinerModel>
{
	public OutlinerPanel()
	{
		ViewModel = new OutlinerModel();

		this.WhenActivated(d =>
		{

		});

		InitializeComponent();
	}
}