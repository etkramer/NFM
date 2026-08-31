using ReactiveUI;
using ReactiveUI.Primitives.Disposables;
using ReactiveUI.Reactive;

namespace NFM;

// ReactiveUI offers both Action<IDisposable> and MultipleDisposable overloads of WhenActivated, which makes
// an untyped lambda ambiguous. These live closer than ViewForMixins, so they win overload resolution.
// DisposeWith is declared here too - importing ReactiveUI.Primitives for it would collide with System.Reactive.Linq.
public static class ActivationExt
{
	public static void WhenActivated(this IActivatableView view, Action<MultipleDisposable> block)
		=> ViewForMixins.WhenActivated(view, block);

	public static void WhenActivated(this IActivatableViewModel viewModel, Action<MultipleDisposable> block)
		=> ViewForMixins.WhenActivated(viewModel, block);

	public static T DisposeWith<T>(this T disposable, MultipleDisposable disposables) where T : IDisposable
	{
		disposables.Add(disposable);
		return disposable;
	}
}
