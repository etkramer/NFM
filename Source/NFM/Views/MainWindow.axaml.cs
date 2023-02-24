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
		TabGroup group1 = new TabGroup();
		group1.Add(new ViewportPanel());
		dockspace.Dock(group1, null);

		// Create group (outliner panel).
		TabGroup group2 = new TabGroup();
		group2.Add(new OutlinerPanel());
		dockspace.Dock(group2, group1, DockPosition.Left, 0.11f);

		// Create group (inspector panel).
		TabGroup group3 = new TabGroup();
		group3.Add(new InspectorPanel());
		dockspace.Dock(group3, group1, DockPosition.Right, 0.13f);

		// Create group (library).
		TabGroup group4 = new TabGroup();
		group4.Add(new ProfilerPanel());
		group4.Add(new LibraryPanel());
		dockspace.Dock(group4, group1, DockPosition.Bottom, 0.30f);

		// Create group (viewport 2).
		/*TabGroup group5 = new TabGroup();
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