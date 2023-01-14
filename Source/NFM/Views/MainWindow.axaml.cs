using System;
using System.ComponentModel;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using ReactiveUI;

namespace NFM.Frontend
{
	public partial class MainWindow : ReactiveWindow<MainWindowModel>
	{
		public const bool UseQuitDialog = false;

		public MainWindow()
		{
			ViewModel = new MainWindowModel();

			this.WhenActivated(d =>
			{
				
			});

			InitializeComponent();
		}

		protected override void OnOpened(EventArgs e)
		{
			RestorePanels();

			// Show landing dialog.
			FrontendHelpers.InvokeHandled(Project.Reset);

			// Begin game loop.
			DispatcherTimer.Run(() => { return FrontendHelpers.InvokeHandled(Engine.Update); }, TimeSpan.Zero, DispatcherPriority.Render);

			base.OnOpened(e);
		}

		public void RestorePanels()
		{
			// Create group (viewport 1).
			TabGroup group1 = new TabGroup();
			ToolPanel.Spawn<ViewportPanel>(group1);
			dockspace.Dock(group1, null);

			// Create group (scene panel).
			/*TabGroup group2 = new TabGroup();
			ToolPanel.Spawn<ScenePanel>(group2);
			dockspace.Dock(group2, group1, DockPosition.Left, 0.11f);

			// Create group (inspector panel).
			TabGroup group3 = new TabGroup();
			ToolPanel.Spawn<InspectorPanel>(group3);
			dockspace.Dock(group3, group1, DockPosition.Right, 0.14f);

			// Create group (library).
			/*TabGroup group4 = new TabGroup();
			ToolPanel.Spawn<LibraryPanel>(group4);
			dockspace.Dock(group4, group1, DockPosition.Bottom, 0.30f);*/

			// Create group (viewport 2).
			/*TabGroup group5 = new TabGroup();
			ToolPanel.Spawn<ViewportPanel>(group5);
			dockspace.Dock(group5, group1, DockPosition.Right, 0.4f);*/
		}

		private bool isQuitConfirmed = false;
		protected override void OnClosing(CancelEventArgs e)
		{
			if (UseQuitDialog)
			{
				e.Cancel = !isQuitConfirmed;

				new Dialog("Quit?", "Are you sure you want to quit? All unsaved changes will be lost.")
					.Button("Quit", (o) => { isQuitConfirmed = true; Close(); })
					.Button("Cancel", (o) => o.Close())
					.Show();
			}

			base.OnClosing(e);
		}
	}
}