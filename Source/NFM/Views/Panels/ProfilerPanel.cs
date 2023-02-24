using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using ReactiveUI;

namespace NFM;

public partial class ProfilerPanel : ToolPanel, IActivatableView
{
	public ProfilerPanel()
	{
		Title = "Profiler";
		Background = this.GetResourceBrush("ThemeControlLowBrush");

		this.WhenActivated(disposables =>
		{
			Observable.Interval(TimeSpan.FromSeconds(1), AvaloniaScheduler.Instance)
				.StartWith(0)
				.Subscribe(o => InvalidateVisual())
				.DisposeWith(disposables);
		});
	}

	public override void Render(DrawingContext context)
	{
		base.Render(context);
	}
}