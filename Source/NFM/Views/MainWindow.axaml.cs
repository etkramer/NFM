using System;
using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using ReactiveUI;

namespace NFM;

public partial class MainWindow : ReactiveWindow<MainWindowModel>
{
	public const bool UseQuitDialog = false;
	public static MainWindow Instance { get; private set; }

	public MainWindow()
	{
		Instance = this;
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
		DockGroup group1 = new DockGroup();
		group1.Add(new ViewportPanel());
		dockspace.Dock(group1, null);

		// Create group (outliner panel).
		DockGroup group2 = new DockGroup();
		group2.Add(new OutlinerPanel());
		dockspace.Dock(group2, group1, DockPosition.Right, 0.13f);

		// Create group (inspector panel).
		DockGroup group3 = new DockGroup();
		group3.Add(new InspectorPanel());
		dockspace.Dock(group3, group2, DockPosition.Bottom, 0.75f);

		// Create group (library).
		/*DockGroup group4 = new DockGroup();
		group4.Add(new ProfilerPanel());
		group4.Add(new LibraryPanel());
		dockspace.Dock(group4, group1, DockPosition.Bottom, 0.30f);

		// Create group (viewport 2).
		DockGroup group5 = new DockGroup();
		group5.Add(new ViewportPanel());
		dockspace.Dock(group5, group1, DockPosition.Right, 0.3f);*/
	}

	bool isQuitConfirmed = false;
	protected override void OnClosing(WindowClosingEventArgs e)
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