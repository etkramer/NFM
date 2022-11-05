using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Engine.Editor;
using Engine.World;

namespace Engine.Frontend
{
	public partial class ScenePanel : ToolPanel
	{
		[Notify] public ReadOnlyObservableCollection<Node> Nodes => Scene.Main?.RootNodes;

		public ScenePanel()
		{
			DataContext = this;
			InitializeComponent();

			// Subscribe to property changed notifications in case the scene changes (i.e. new project).
			StaticNotify.Subscribe(typeof(Scene), nameof(Scene.Main), () => (this as INotify).Raise(nameof(Nodes)));

			// Bind TreeView to editor selection. TODO: Figure out how to move this to XAML.
			sceneTree.Bind(TreeView.SelectedItemsProperty, new Binding("selected", BindingMode.Default) { Source = typeof(Selection) });
		}

		private void OnAddPressed(object sender, RoutedEventArgs args)
		{
			FlyoutBase.ShowAttachedFlyout(sender as Control);
		}

		private void OnRemovePressed(object sender, RoutedEventArgs args)
		{
			var nodes = sceneTree.SelectedItems.Cast<Node>();

			foreach (var node in nodes.ToArray())
			{
				node.Dispose();
			}
		}

		private void OnAddNodePressed(string type)
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
}
