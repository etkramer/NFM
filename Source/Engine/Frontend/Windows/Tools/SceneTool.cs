using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Markup.Xaml.Templates;
using Engine.Editor;
using Engine.World;
using ISelectable = Engine.Editor.ISelectable;

namespace Engine.Frontend
{
	public partial class SceneTool : ToolWindow
	{
		[Notify] public ReadOnlyObservableCollection<Actor> SceneActors => Scene.Main?.Actors;

		public SceneTool()
		{
			StaticNotify.Subscribe(typeof(Scene), nameof(Scene.Main), () => (this as INotify).Raise(nameof(SceneActors)));

			TreeView sceneTree = null;
			DataContext = this;
			Title = "Scene";
			Content = new Grid()
				.Background(this.GetResourceBrush("ControlBackground"))
				.Children(
					new TreeView()
						.Background("#00000000")
						.BorderWidth(0)
						.Margin(4)
						.SelectionMode(SelectionMode.Multiple)
						.Items(nameof(SceneActors), BindingMode.Default)
						.ItemTemplate(new FuncTreeDataTemplate<Actor>((o, e) => BuildItemContent(), (o) => o.Children))
						.With(o => sceneTree = o)
						.With(o => o.PointerPressed += (o, e) => Selection.DeselectAll())
				);

			// Bind SceneTree to selection property.
			sceneTree.Bind(TreeView.SelectedItemsProperty, new Binding("selected", BindingMode.Default) { Source = typeof(Selection) });
		}

		private Control BuildItemContent()
		{
			return new TextBlock().Text(nameof(Actor.Name), BindingMode.Default);
		}
	}
}