using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using Avalonia.Input;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace NFM;

public unsafe class HwndControl : NativeControlHost
{
	private static List<HwndControl> hosts = new();
	private static bool isInFrame = false;

	/// <summary>
	/// Invoked once per frame, from the WM_PAINT of whichever host is currently driving the loop.
	/// </summary>
	public static event Action OnFrame = delegate{};

	public event Action<Vector2i> OnResize = delegate{};
	public event Action OnOpen = delegate{};
	public event Action OnClose = delegate{};

	public IntPtr Hwnd { get; private set; }
	private bool hasValidMeasure = false;

	private WNDPROC subclassProc;
	private nint baseProc;

	private IntPtr hostHwnd;
	private WNDPROC hostSubclassProc;
	private nint hostBaseProc;

	protected unsafe override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
	{
		// Create Hwnd.
		IPlatformHandle platformHandle = base.CreateNativeControlCore(parent);
		Hwnd = platformHandle.Handle;

		// Pass hit-testing through to Avalonia, which handles viewport input itself.
		// Avalonia parents us under an intermediate holder window, which would otherwise catch the hit first.
		hostHwnd = parent.Handle;
		hostSubclassProc = HostSubclassProc;
		hostBaseProc = PInvoke.SetWindowLongPtr((HWND)hostHwnd, WINDOW_LONG_PTR_INDEX.GWLP_WNDPROC, Marshal.GetFunctionPointerForDelegate(hostSubclassProc));

		subclassProc = SubclassProc;
		baseProc = PInvoke.SetWindowLongPtr((HWND)Hwnd, WINDOW_LONG_PTR_INDEX.GWLP_WNDPROC, Marshal.GetFunctionPointerForDelegate(subclassProc));

		// Make it clear that the bounds are invalid, and request a new measurement.
		Bounds = new(0, 0, -1, -1);
		InvalidateArrange();

		hosts.Add(this);
		RequestFrame();

		if (hasValidMeasure)
		{
			OnOpen.Invoke();
		}

		return platformHandle;
	}

	protected override void DestroyNativeControlCore(IPlatformHandle control)
	{
		// Cleanup viewport and swapchain.
		if (hasValidMeasure)
		{
			OnClose.Invoke();
		}

		if (baseProc != 0)
		{
			PInvoke.SetWindowLongPtr((HWND)Hwnd, WINDOW_LONG_PTR_INDEX.GWLP_WNDPROC, baseProc);
			baseProc = 0;
			subclassProc = null;
		}

		if (hostBaseProc != 0)
		{
			PInvoke.SetWindowLongPtr((HWND)hostHwnd, WINDOW_LONG_PTR_INDEX.GWLP_WNDPROC, hostBaseProc);
			hostBaseProc = 0;
			hostSubclassProc = null;
		}

		hosts.Remove(this);
		RequestFrame();

		base.DestroyNativeControlCore(control);
	}

	/// <summary>
	/// Queues the next frame. WM_PAINT is only generated once the message queue is otherwise empty,
	/// so input and dispatcher work always take precedence over the game loop.
	/// </summary>
	private static void RequestFrame()
	{
		if (hosts.Count > 0)
		{
			PInvoke.InvalidateRect((HWND)hosts[0].Hwnd, (RECT*)null, false);
		}
	}

	private LRESULT HostSubclassProc(HWND hwnd, uint msg, WPARAM wParam, LPARAM lParam)
	{
		if (msg == PInvoke.WM_NCHITTEST)
		{
			return new LRESULT((int)PInvoke.HTTRANSPARENT);
		}

		return ((delegate* unmanaged[Stdcall]<HWND, uint, WPARAM, LPARAM, LRESULT>)hostBaseProc)(hwnd, msg, wParam, lParam);
	}

	private LRESULT SubclassProc(HWND hwnd, uint msg, WPARAM wParam, LPARAM lParam)
	{
		if (msg == PInvoke.WM_NCHITTEST)
		{
			return new LRESULT((int)PInvoke.HTTRANSPARENT);
		}

		if (msg == PInvoke.WM_PAINT && hosts.Count > 0 && hosts[0] == this)
		{
			PInvoke.ValidateRect(hwnd, (RECT*)null);

			// Guard against reentrancy, in case a tick pumps messages itself (dialogs, blocking waits).
			if (!isInFrame)
			{
				isInFrame = true;

				try
				{
					OnFrame.Invoke();
				}
				finally
				{
					isInFrame = false;
				}
			}

			RequestFrame();
			return new LRESULT(0);
		}

		return ((delegate* unmanaged[Stdcall]<HWND, uint, WPARAM, LPARAM, LRESULT>)baseProc)(hwnd, msg, wParam, lParam);
	}

	private Vector2i lastSize = Vector2i.Zero;
	protected override Size ArrangeOverride(Size finalSize)
	{
		Size arrangeResult = base.ArrangeOverride(finalSize);
		Vector2i size = new((int)arrangeResult.Width, (int)arrangeResult.Height);

		if (!hasValidMeasure)
		{
			hasValidMeasure = true;
			OnOpen.Invoke();
		}

		if (size != lastSize)
		{
			// Resize the swapchain if needed.
			OnResize.Invoke(size);
		}

		lastSize = size;

		return arrangeResult;
	}
}
