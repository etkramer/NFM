using System.Reactive.Linq;
using NFM.GPU;
using ReactiveUI.Reactive;

namespace NFM;

public class ViewportModel : IActivatableViewModel
{
	public ViewModelActivator Activator { get; } = new();

	[Notify] public string FrameTimeDisplay { get; private set; }
	[Notify] public string MemoryDisplay { get; private set; }

	public ViewportModel()
	{
		this.WhenActivated(disposables =>
		{
			Observable.Interval(TimeSpan.FromSeconds(1))
				.StartWith(0)
				.Select(o => $"Memory: {Environment.WorkingSet / 1024 / 1024}MB")
				.Subscribe(o => MemoryDisplay = o)
				.DisposeWith(disposables);

			Observable.Interval(TimeSpan.FromSeconds(0.1))
				.StartWith(0)
				.Select(o => $"Frametime: {Metrics.FrameTime * 1000:0.00}ms")
				.Subscribe(o => FrameTimeDisplay = o)
				.DisposeWith(disposables);
		});
	}
}