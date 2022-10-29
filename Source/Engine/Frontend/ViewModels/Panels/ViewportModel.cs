using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Engine.GPU;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Engine.Frontend
{
	public class ViewportModel : ReactiveObject, IActivatableViewModel
	{
		public ViewModelActivator Activator { get; } = new();

		[ObservableAsProperty]
		public string FrameTimeDisplay { get; }

		[ObservableAsProperty]
		public string MemoryDisplay { get; }

		public ViewportModel()
		{
			this.WhenActivated(disposables =>
			{
				Observable.Interval(TimeSpan.FromSeconds(1))
					.StartWith(0)
					.Select(o => $"Memory: {Environment.WorkingSet / 1024 / 1024}MB")
					.ToPropertyEx(this, o => o.MemoryDisplay)
					.DisposeWith(disposables);

				Observable.Interval(TimeSpan.FromSeconds(0.1))
					.StartWith(0)
					.Select(o => $"Frametime: {Metrics.FrameTime * 1000:0.00}ms")
					.ToPropertyEx(this, o => o.FrameTimeDisplay)
					.DisposeWith(disposables);
			});
		}
	}
}