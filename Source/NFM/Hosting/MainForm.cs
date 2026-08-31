using System.Diagnostics;
using NFM.Components;
using Windows.Win32;
using Windows.Win32.Foundation;

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
	private bool isInFrame;

	protected override void OnShown(EventArgs e)
	{
		base.OnShown(e);
		RequestFrame();
	}

	/// <summary>
	/// Queues the next frame. WM_PAINT is only generated once the message queue is otherwise empty,
	/// so input and webview work always take precedence over the game loop.
	/// </summary>
	private unsafe void RequestFrame()
	{
		PInvoke.InvalidateRect((HWND)Handle, (RECT*)null, false);
	}

	private void Frame()
	{
		FrontendHelpers.InvokeHandled(Engine.Update);

		if (clock.Elapsed - lastTick >= TickInterval)
		{
			lastTick = clock.Elapsed;
			FrontendHelpers.InvokeHandled(Component.Tick);
		}
	}

	protected override void WndProc(ref Message m)
	{
		if (m.Msg == PInvoke.WM_PAINT)
		{
			// Guard against reentrancy, in case a frame pumps messages itself (dialogs, blocking waits).
			if (!isInFrame)
			{
				isInFrame = true;

				try
				{
					Frame();
				}
				finally
				{
					isInFrame = false;
				}
			}

			base.WndProc(ref m);

			// Once the app has faulted, stop self-invalidating - the error dialog needs the message queue.
			if (!FrontendHelpers.HasFaulted)
			{
				RequestFrame();
			}

			return;
		}

		base.WndProc(ref m);
	}
}
