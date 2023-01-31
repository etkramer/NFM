using System;
using Avalonia.Controls.Primitives;
using Avalonia.Controls;
using Avalonia.Interactivity;
using NFM.World;
using ReactiveUI;
using Avalonia.Controls.Models.TreeDataGrid;
using ReactiveUI.Fody.Helpers;

namespace NFM;

public class OutlinerModel : ReactiveObject, IActivatableViewModel
{
	public ViewModelActivator Activator { get; } = new();

	[Reactive]
	public ITreeDataGridSource<Node> NodesSource { get; private set; }

	public OutlinerModel()
	{
		NodesSource = GetSource(Scene.Main.RootNodes);

		// Subscribe to property changed notifications in case the scene changes (i.e. new project).
		StaticNotify.Subscribe(typeof(Scene), nameof(Scene.Main), () =>
		{
			NodesSource = GetSource(Scene.Main.RootNodes);
		});
	}

	ITreeDataGridSource<Node> GetSource(IEnumerable<Node> nodes)
	{
		var source = new HierarchicalTreeDataGridSource<Node>(nodes)
		{
			Columns =
			{
				new HierarchicalExpanderColumn<Node>(new TextColumn<Node, string>("Name", o => o.Name, new GridLength(1.5f, GridUnitType.Star)), o => o.Children),
				new TextColumn<Node, string>("Type", o => o.GetType().Name, new GridLength(1, GridUnitType.Star)),
			},
		};

		// Keep selection in sync
		source.RowSelection.SelectionChanged += (o, e) =>
		{
			if (e.DeselectedItems != null)
			{
				Selection.Deselect(e.DeselectedItems.ToArray());
			}
			if (e.SelectedItems != null)
			{
				Selection.Select(e.SelectedItems.ToArray());
			}
		};

		source.RowSelection.SingleSelect = false;
		return source;
	}

	public void OnAddPressed() {}
	public void OnAddPressed(object sender, RoutedEventArgs args)
	{
		FlyoutBase.ShowAttachedFlyout(sender as Control);
	}

	public void OnAddModelPressed() => new ModelNode(null);
	public void OnAddCameraPressed() => new CameraNode(null);
	public void OnAddPointLightPressed() => new PointLightNode(null);
}