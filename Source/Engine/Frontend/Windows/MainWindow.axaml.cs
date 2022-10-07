using System;
using System.ComponentModel;
using System.Runtime.ExceptionServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Engine.Editor;
using Engine.World;

namespace Engine.Frontend.Controls
{
	internal partial class MainWindow : Window
	{
		public static MainWindow Instance { get; private set; }

		public MainWindow()
		{
			Instance = this;
			InitializeComponent();
			DataContext = this;
		}

		protected override void OnOpened(EventArgs args)
		{
			// Create group (viewport 1).
			TabGroup group1 = new TabGroup();
			ToolPanel.Spawn<ViewportPanel>(group1);
			dockspace.Dock(group1, null);

			// Create group (scene panel).
			TabGroup group2 = new TabGroup();
			ToolPanel.Spawn<ScenePanel>(group2);
			dockspace.Dock(group2, group1, DockPosition.Left, 0.11f);

			// Create group (inspector panel).
			TabGroup group3 = new TabGroup();
			ToolPanel.Spawn<InspectorPanel>(group3);
			dockspace.Dock(group3, group1, DockPosition.Right, 0.14f);

			// Create group (timeline).
			/*TabGroup group4 = new TabGroup();
			ToolPanel.Spawn<TimelinePanel>(group4);
			dockspace.Dock(group4, group1, DockPosition.Bottom, 0.30f);*/

			// Create group (viewport 2).
			TabGroup group5 = new TabGroup();
			ToolPanel.Spawn<ViewportPanel>(group5);
			dockspace.Dock(group5, group1, DockPosition.Right, 0.4f);

			// Show landing dialog.
			if (LandingDialog.ShowOnStartup)
			{
				new LandingDialog().Show();
			}
			else
			{
				FrontendHelpers.InvokeHandled(() => Project.Reset());
			}

			// Begin game loop.
			DispatcherTimer.Run(() => { return FrontendHelpers.InvokeHandled(Game.Update); }, TimeSpan.Zero, DispatcherPriority.Render);
		}

		public void NewPressed()
		{
			Project.Reset();
		}

		public async void OpenPressed()
		{
			string openPath = await Dialog.ShowOpenDialog(this, new FileFilter("Project", "json"));

			if (openPath != null)
			{
				Project.Load(openPath);
			}
		}

		public void SavePressed()
		{
			if (Project.Path == null)
			{
				SaveAsPressed();
				return;
			}

			Project.Save(null);
		}

		public async void SaveAsPressed()
		{
			string savePath = await Dialog.ShowSaveDialog(this, new FileFilter("Project", "json"));

			if (savePath != null)
			{
				Project.Save(savePath);
			}
		}

		private bool isQuitConfirmed = false;

		protected override void OnClosing(CancelEventArgs e)
		{
			/*e.Cancel = !isQuitConfirmed;

			new Popup("Quit?", "Are you sure you want to quit? All unsaved changes will be lost.")
				.Button("Quit", (o) => { isQuitConfirmed = true; Close(); })
				.Button("Cancel", (o) => o.Close())
				.Open();*/

			base.OnClosing(e);
		}

		public void QuitPressed() => Close();
		public void UndoPressed() => Command.Undo();
		public void RedoPressed() => Command.Redo();
		public void DeletePressed() => Selection.Selected.OfType<Node>().ToArray().ForEach(o => o.Dispose());
	}
}