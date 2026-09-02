using System.Diagnostics;
using NFM.Components;

namespace NFM.Hosting;

/// <summary>
/// The editor window. Beyond hosting the page, it owns the frame loop that drives the engine.
/// </summary>
sealed class MainForm : ComponentForm<MainPage>
{
	/// <summary>
	/// How often component state is polled for changes. The render loop runs far faster than this,
	/// and the UI has no reason to keep up with it.
	/// </summary>
	private static readonly TimeSpan TickInterval = TimeSpan.FromSeconds(1 / 60d);

	private readonly Stopwatch clock = Stopwatch.StartNew();
	private TimeSpan lastTick;

	/// <summary>
	/// Advances the engine by a frame. Driven by the loop in <see cref="Program"/>, which pumps the
	/// message queue dry first - input and webview work always take precedence over the game loop.
	/// </summary>
	internal void Frame()
	{
		Engine.Update();

		if (clock.Elapsed - lastTick >= TickInterval)
		{
			lastTick = clock.Elapsed;
			Component.Tick();
		}
	}
}
