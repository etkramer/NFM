using System;
using System.Runtime.ExceptionServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;

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
			// Create group.
			TabGroup group1 = new TabGroup();
			ToolPanel.Spawn<ScenePanel>(group1);
			dockspace.Dock(group1, null);

			// Create group.
			TabGroup group2 = new TabGroup();
			ToolPanel.Spawn<InspectorPanel>(group2);
			dockspace.Dock(group2, group1, DockPosition.Right, 0.88f);

			// Create group (timeline).
			TabGroup group3 = new TabGroup();
			ToolPanel.Spawn<TimelinePanel>(group3);
			dockspace.Dock(group3, group2, DockPosition.Left, 0.88f);

			// Create group (viewport 1).
			TabGroup group4 = new TabGroup();
			ToolPanel.Spawn<ViewportPanel>(group4);
			dockspace.Dock(group4, group3, DockPosition.Top, 0.75f);

			// Create group (viewport 2).
			/*TabGroup group5 = new TabGroup();
			ToolPanel.Spawn<ViewportPanel>(group5);
			dockspace.Dock(group5, group4, DockPosition.Right, 0.5f);*/

			// Show landing dialog.
			if (LandingDialog.ShowOnStartup)
			{
				new LandingDialog().Show();
			}
			else
			{
				Project.Create();
			}

			// Begin game loop.
			DispatcherTimer.Run(() =>
			{
				try
				{
					Task.Run(() =>
					{
						Game.Update();
					}).Wait();
					
					return true;
				}
				catch (Exception e)
				{
					new ExceptionDialog(ExceptionDispatchInfo.Capture(e)).Show();
					return false;
				}
			},
			TimeSpan.Zero, DispatcherPriority.Render);
		}

		public void NewPressed()
		{
			Project.Create();
		}

		public async void OpenPressed()
		{
			string openPath = await Dialog.ShowOpenDialog(this, new FileFilter("Project file", "project"));

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
			string savePath = await Dialog.ShowSaveDialog(this, new FileFilter("Project file", "project"));

			if (savePath != null)
			{
				Project.Save(savePath);
			}
		}

		public void QuitPressed()
		{
			Close();
		}

		public void UndoPressed() => Command.Undo();
		public void RedoPressed() => Command.Redo();
	}
}