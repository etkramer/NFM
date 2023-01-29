using System;
using Avalonia.Controls.Primitives;
using Avalonia.Controls;
using Avalonia.Interactivity;
using NFM.World;
using ReactiveUI;
using System.Reactive.Disposables;
using Avalonia.Controls.Models.TreeDataGrid;

namespace NFM;

public class OutlinerModel : ReactiveObject, IActivatableViewModel
{
	public ViewModelActivator Activator { get; } = new();

	public OutlinerModel()
	{

	}

	public void OnAddPressed() {}
	public void OnAddPressed(object sender, RoutedEventArgs args)
	{
		FlyoutBase.ShowAttachedFlyout(sender as Control);
	}

	public void OnAddNodePressed() {}
	public void OnAddNodePressed(string type)
	{
		if (type == "Model")
		{
			new ModelNode(null);
		}
		else if (type == "Camera")
		{
			new CameraNode(null);
		}
		else if (type == "PointLight")
		{
			new PointLightNode(null);
		}
	}
}