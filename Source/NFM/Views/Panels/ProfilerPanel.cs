using System.Reactive.Linq;
using Avalonia.Media;
using ReactiveUI.Avalonia.Reactive;
using ReactiveUI;
using ReactiveUI.Reactive;

namespace NFM;

public partial class ProfilerPanel : ToolPanel, IActivatableView
{
	public ProfilerPanel()
	{
		Title = "Profiler";
		Background = this.GetResourceBrush("ThemeControlLowBrush");

		this.WhenActivated(disposables =>
		{
			Observable.Interval(TimeSpan.FromSeconds(1), ReactiveUI.Primitives.Reactive.Concurrency.AvaloniaScheduler.Instance)
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