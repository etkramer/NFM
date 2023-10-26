using Avalonia.Controls.Primitives;
using Avalonia.Controls;
using Avalonia.Interactivity;
using NFM.World;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.Specialized;

namespace NFM;

public class OutlinerModel : ReactiveObject, IActivatableViewModel
{
	public ViewModelActivator Activator { get; } = new();

	[Reactive]
	public IEnumerable<Node> NodesSource { get; private set; }

	public IEnumerable<Node> SelectedNodes { get; }

	public OutlinerModel()
	{
		NodesSource = Scene.Main.RootNodes;

		SelectedNodes = new ObservableCollection<Node>();
		(SelectedNodes as INotifyCollectionChanged).CollectionChanged += (o, e) =>
		{
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				Selection.Select(e.NewItems.Cast<ISelectable>());
			}
			else if (e.Action == NotifyCollectionChangedAction.Remove)
			{
				Selection.Deselect(e.OldItems.Cast<ISelectable>());
			}
			else if (e.Action == NotifyCollectionChangedAction.Reset)
			{
				Selection.DeselectAll();
			}
			else if (e.Action == NotifyCollectionChangedAction.Replace)
			{
				Selection.Deselect(e.OldItems.Cast<ISelectable>());
				Selection.Select(e.NewItems.Cast<ISelectable>());
			}
		};

		// Subscribe to property changed notifications in case the scene changes (i.e. new project).
		StaticNotify.Subscribe(typeof(Scene), nameof(Scene.Main), () =>
		{
			(SelectedNodes as IList<Node>).Clear();
			NodesSource = Scene.Main.RootNodes;
		});
	}

	void OnSelectionChanged(object sender, SelectionChangedEventArgs args)
	{
		if (args.RemovedItems != null)
		{
			Selection.Deselect(args.RemovedItems.Cast<ISelectable>());
		}
		if (args.AddedItems != null)
		{
			Selection.Select(args.RemovedItems.Cast<ISelectable>());
		}
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