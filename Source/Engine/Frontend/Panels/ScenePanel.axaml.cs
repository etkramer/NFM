using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Interactivity;
using Engine.Editor;
using Engine.World;

namespace Engine.Frontend
{
	public partial class ScenePanel : ToolPanel
	{
		[Notify] public ReadOnlyObservableCollection<Actor> SceneActors => Scene.Main?.Actors;

		public ScenePanel()
		{
			DataContext = this;
			InitializeComponent();

			// Subscribe to property changed notifications in case the scene changes (i.e. new project).
			StaticNotify.Subscribe(typeof(Scene), nameof(Scene.Main), () => (this as INotify).Raise(nameof(SceneActors)));

			// Bind TreeView to editor selection. TODO: Figure out how to move this to XAML.
			sceneTree.Bind(TreeView.SelectedItemsProperty, new Binding("selected", BindingMode.Default) { Source = typeof(Selection) });
		}

		private void OnAddPressed(object sender, RoutedEventArgs args)
		{
			FlyoutBase.ShowAttachedFlyout(sender as Control);
		}

		private void OnRemovePressed(object sender, RoutedEventArgs args)
		{
			var actors = Selection.Selected.OfType<Actor>();
			foreach (var actor in actors.ToArray())
			{
				actor.Dispose();
			}
		}

		private void OnCreatePressed(string type)
		{
			if (type == "Model")
			{
				new ModelActor().Spawn();
			}
			else if (type == "Camera")
			{
				new CameraActor().Spawn();
			}
			else if (type == "PointLight")
			{
				new PointLightActor().Spawn();
			}
		}
	}
}
