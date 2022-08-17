using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using Avalonia.Data;
using Avalonia.Layout;
using Engine.GPU;
using Engine.Rendering;
using WinApi.User32;
using WndProc = Avalonia.Win32.Interop.UnmanagedMethods.WndProc;
using Avalonia.Win32;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Threading;
using Avalonia.Interactivity;

namespace Engine.Frontend
{
	public class ViewportTool : ToolWindow
	{
		[Notify] string frameTime => $"Frametime: {frameTimeAverager.Result.ToString("0.00")}ms";
		[Notify] string memory => $"Memory: {Environment.WorkingSet / 1024 / 1024}MB";
		private Averager frameTimeAverager = new Averager(100);

		public ViewportTool()
		{
			// Update frametime.
			Graphics.OnFrameStart += () => frameTimeAverager.AddValue(Graphics.FrameTime * 1000);
			(frameTimeAverager as INotify).Subscribe(nameof(Averager.Result), () => (this as INotify).Raise(nameof(frameTime)));

			// Update memory.
			Graphics.OnFrameStart += () => (this as INotify).Raise(nameof(memory));

			// Build tool contents.
			DataContext = this;
			Title = "Viewport";
			Content = new Grid()
				.Rows("*, 26")
				.Children(
					new ViewportHost(),
					new StackPanel()
						.Row(1)
						.Margin(10, 0)
						.Spacing(10)
						.HorizontalAlignment(HorizontalAlignment.Right)
						.Orientation(Orientation.Horizontal)
						.Children(
							new TextBlock()
								.VerticalAlignment(VerticalAlignment.Center)
								.HorizontalAlignment(HorizontalAlignment.Right)
								.Text(nameof(memory), BindingMode.Default),
							new TextBlock()
								.VerticalAlignment(VerticalAlignment.Center)
								.HorizontalAlignment(HorizontalAlignment.Right)
								.Text(nameof(frameTime), BindingMode.Default)
						)
				);
		}
	}

	public class ViewportHost : Panel
	{
		public event Action OnOpen = delegate{};
		public event Action OnClose = delegate{};
		public event Action<Vector2i> OnResize = delegate{};

		public Swapchain Swapchain { get; private set; }
		private Viewport viewport;

		public ViewportHost()
		{
			NativeControlHostEx nativeControl = new();

			IsHitTestVisible = true;
			Focusable = true;
			this.Background("Transparent");
			this.Children(new NativeControlHostEx().With(o => nativeControl = o));

			// Setup open/close/resize callbacks.
			nativeControl.OnOpen += () =>
			{
				Swapchain = new Swapchain(nativeControl.Hwnd, new(32, 32));
				viewport = new Viewport(this);

				OnOpen.Invoke();
			};
			nativeControl.OnClose += () =>
			{
				viewport.Dispose();
				Swapchain.Dispose();

				OnClose.Invoke();
			};
			nativeControl.OnResize += (size) =>
			{
				Swapchain.Resize(size);

				OnResize.Invoke(size);
			};
		}

		protected override void OnPointerMoved(PointerEventArgs e)
		{
			PointerPointProperties props = e.GetCurrentPoint(this).Properties;

			if (props.IsRightButtonPressed)
			{
				//Debug.Log("Dragged");
			}

			base.OnPointerMoved(e);
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp(e);
		}
	}

	public unsafe class NativeControlHostEx : NativeControlHost
	{
		private static List<NativeControlHostEx> hosts = new();

		public event Action<Vector2i> OnResize = delegate{};
		public event Action OnOpen = delegate{};
		public event Action OnClose = delegate{};

		public IntPtr Hwnd { get; private set; }
		private bool hasValidMeasure = false;

		#region Platform
		// Override WndProc to handle input events.
		private static WndProc baseWndProc;
		private static WndProc overrideWndProc = WndProcOverride;
		private static unsafe IntPtr WndProcOverride(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam)
		{
			NativeControlHostEx host = hosts.Find(o => o.Hwnd == hwnd);

			// Early out if the host can't be found.
			if (host == null || host?.VisualRoot == null)
			{
				return baseWndProc(hwnd, msg, wParam, lParam);
			}

			// Grab the input callback.
			Action<RawInputEventArgs> input = (host.VisualRoot as Window).PlatformImpl.Input;
			var inputRoot = host.VisualRoot as IInputRoot;
			var mouseDevice = inputRoot.MouseDevice;
			var keyboardDevice = AvaloniaLocator.Current.GetService<IKeyboardDevice>();

			// Override WndProc.
			switch ((WM)msg)
			{
				case WM.MOUSEMOVE:
				{
					int x = (short)(int)lParam + (int)host.TransformedBounds.Value.Clip.Left;
					int y = (short)(((int)lParam) >> 16) + (int)host.TransformedBounds.Value.Clip.Top;
					input.Invoke(new RawPointerEventArgs(mouseDevice, 0, host.VisualRoot as Window, RawPointerEventType.Move, new Point(x, y), GetModifiers((ulong)wParam)));
					break;
				}
				case WM.LBUTTONDOWN:
				{
					int x = (short)(int)lParam + (int)host.TransformedBounds.Value.Clip.Left;
					int y = (short)(((int)lParam) >> 16) + (int)host.TransformedBounds.Value.Clip.Top;
					input.Invoke(new RawPointerEventArgs(mouseDevice, 0, host.VisualRoot as Window, RawPointerEventType.LeftButtonDown, new Point(x, y), GetModifiers((ulong)wParam)));
					break;
				}
				case WM.RBUTTONDOWN:
				{
					int x = (short)(int)lParam + (int)host.TransformedBounds.Value.Clip.Left;
					int y = (short)(((int)lParam) >> 16) + (int)host.TransformedBounds.Value.Clip.Top;
					input.Invoke(new RawPointerEventArgs(mouseDevice, 0, host.VisualRoot as Window, RawPointerEventType.RightButtonDown, new Point(x, y), GetModifiers((ulong)wParam)));
					break;
				}
				case WM.MBUTTONDOWN:
				{
					int x = (short)(int)lParam + (int)host.TransformedBounds.Value.Clip.Left;
					int y = (short)(((int)lParam) >> 16) + (int)host.TransformedBounds.Value.Clip.Top;
					input.Invoke(new RawPointerEventArgs(mouseDevice, 0, host.VisualRoot as Window, RawPointerEventType.MiddleButtonDown, new Point(x, y), GetModifiers((ulong)wParam)));
					break;
				}
				case WM.LBUTTONUP:
				{
					int x = (short)(int)lParam + (int)host.TransformedBounds.Value.Clip.Left;
					int y = (short)(((int)lParam) >> 16) + (int)host.TransformedBounds.Value.Clip.Top;
					input.Invoke(new RawPointerEventArgs(mouseDevice, 0, host.VisualRoot as Window, RawPointerEventType.LeftButtonUp, new Point(x, y), GetModifiers((ulong)wParam)));
					break;
				}
				case WM.RBUTTONUP:
				{
					int x = (short)(int)lParam + (int)host.TransformedBounds.Value.Clip.Left;
					int y = (short)(((int)lParam) >> 16) + (int)host.TransformedBounds.Value.Clip.Top;
					input.Invoke(new RawPointerEventArgs(mouseDevice, 0, host.VisualRoot as Window, RawPointerEventType.RightButtonUp, new Point(x, y), GetModifiers((ulong)wParam)));
					break;
				}
				case WM.MBUTTONUP:
				{
					int x = (short)(int)lParam + (int)host.TransformedBounds.Value.Clip.Left;
					int y = (short)(((int)lParam) >> 16) + (int)host.TransformedBounds.Value.Clip.Top;
					input.Invoke(new RawPointerEventArgs(mouseDevice, 0, host.VisualRoot as Window, RawPointerEventType.MiddleButtonUp, new Point(x, y), GetModifiers((ulong)wParam)));
					break;
				}
				case WM.MOUSEWHEEL:
				{
					int x = (short)(int)lParam + (int)host.TransformedBounds.Value.Clip.Left;
					int y = (short)(((int)lParam) >> 16) + (int)host.TransformedBounds.Value.Clip.Top;
					int delta = (short)((long)wParam >> 16);
					new RawMouseWheelEventArgs(mouseDevice, 0, host.VisualRoot as Window, new Point(x, y), new Vector(0, delta), GetModifiers((ulong)wParam));
					break;
				}
				case WM.MOUSEHWHEEL:
				{
					int x = (short)(int)lParam + (int)host.TransformedBounds.Value.Clip.Left;
					int y = (short)(((int)lParam) >> 16) + (int)host.TransformedBounds.Value.Clip.Top;
					int delta = (short)((long)wParam >> 16);
					new RawMouseWheelEventArgs(mouseDevice, 0, host.VisualRoot as Window, new Point(x, y), new Vector(delta, 0), GetModifiers((ulong)wParam));
					break;
				}
				case WM.KEYDOWN:
				{
					new RawKeyEventArgs(keyboardDevice, 0, host.VisualRoot as Window, RawKeyEventType.KeyDown, GetKey((int)wParam), RawInputModifiers.None);
					break;
				}
				case WM.KEYUP:
				{
					new RawKeyEventArgs(keyboardDevice, 0, host.VisualRoot as Window, RawKeyEventType.KeyUp, GetKey((int)wParam), RawInputModifiers.None);
					break;
				}

				default:
					break;
			};

			return baseWndProc(hwnd, msg, wParam, lParam);
		}

		private static Key GetKey(int wParam)
		{
			VirtualKey sourceKey = (VirtualKey)wParam;

			switch (sourceKey)
			{
				case VirtualKey.CANCEL:
					return Key.Cancel;
				case VirtualKey.BACK:
					return Key.Back;
				case VirtualKey.TAB:
					return Key.Tab;
				case VirtualKey.CLEAR:
					return Key.Clear;
				case VirtualKey.RETURN:
					return Key.Return;
				case VirtualKey.CAPITAL:
					return Key.Capital;
				case VirtualKey.ESCAPE:
					return Key.Escape;
				case VirtualKey.SPACE:
					return Key.Space;
				case VirtualKey.PRIOR:
					return Key.PageUp;
				case VirtualKey.NEXT:
					return Key.PageDown;
				case VirtualKey.END:
					return Key.End;
				case VirtualKey.LEFT:
					return Key.Left;
				case VirtualKey.UP:
					return Key.Up;
				case VirtualKey.RIGHT:
					return Key.Right;
				case VirtualKey.DOWN:
					return Key.Down;
				case VirtualKey.PRINT:
					return Key.Print;
				case VirtualKey.INSERT:
					return Key.Insert;
				case VirtualKey.DELETE:
					return Key.Delete;
				case VirtualKey.SNAPSHOT:
					return Key.Snapshot;
				case VirtualKey.D0:
					return Key.D0;
				case VirtualKey.D1:
					return Key.D1;
				case VirtualKey.D2:
					return Key.D2;
				case VirtualKey.D3:
					return Key.D3;
				case VirtualKey.D4:
					return Key.D4;
				case VirtualKey.D5:
					return Key.D5;
				case VirtualKey.D6:
					return Key.D6;
				case VirtualKey.D7:
					return Key.D7;
				case VirtualKey.D8:
					return Key.D8;
				case VirtualKey.D9:
					return Key.D9;
				case VirtualKey.A:
					return Key.A;
				case VirtualKey.B:
					return Key.B;
				case VirtualKey.C:
					return Key.C;
				case VirtualKey.D:
					return Key.D;
				case VirtualKey.E:
					return Key.E;
				case VirtualKey.F:
					return Key.F;
				case VirtualKey.G:
					return Key.G;
				case VirtualKey.H:
					return Key.H;
				case VirtualKey.I:
					return Key.I;
				case VirtualKey.J:
					return Key.J;
				case VirtualKey.K:
					return Key.K;
				case VirtualKey.L:
					return Key.L;
				case VirtualKey.M:
					return Key.M;
				case VirtualKey.N:
					return Key.N;
				case VirtualKey.O:
					return Key.O;
				case VirtualKey.P:
					return Key.P;
				case VirtualKey.Q:
					return Key.Q;
				case VirtualKey.R:
					return Key.R;
				case VirtualKey.S:
					return Key.S;
				case VirtualKey.T:
					return Key.T;
				case VirtualKey.U:
					return Key.U;
				case VirtualKey.V:
					return Key.V;
				case VirtualKey.W:
					return Key.W;
				case VirtualKey.X:
					return Key.X;
				case VirtualKey.Y:
					return Key.Y;
				case VirtualKey.Z:
					return Key.Z;
				case VirtualKey.NUMPAD0:
					return Key.NumPad0;
				case VirtualKey.NUMPAD1:
					return Key.NumPad1;
				case VirtualKey.NUMPAD2:
					return Key.NumPad2;
				case VirtualKey.NUMPAD3:
					return Key.NumPad3;
				case VirtualKey.NUMPAD4:
					return Key.NumPad4;
				case VirtualKey.NUMPAD5:
					return Key.NumPad5;
				case VirtualKey.NUMPAD6:
					return Key.NumPad6;
				case VirtualKey.NUMPAD7:
					return Key.NumPad7;
				case VirtualKey.NUMPAD8:
					return Key.NumPad8;
				case VirtualKey.NUMPAD9:
					return Key.NumPad9;
				case VirtualKey.MULTIPLY:
					return Key.Multiply;
				case VirtualKey.ADD:
					return Key.Add;
				case VirtualKey.SUBTRACT:
					return Key.Subtract;
				case VirtualKey.DIVIDE:
					return Key.Divide;
				case VirtualKey.F1:
					return Key.F1;
				case VirtualKey.F2:
					return Key.F2;
				case VirtualKey.F3:
					return Key.F3;
				case VirtualKey.F4:
					return Key.F4;
				case VirtualKey.F5:
					return Key.F5;
				case VirtualKey.F6:
					return Key.F6;
				case VirtualKey.F7:
					return Key.F7;
				case VirtualKey.F8:
					return Key.F8;
				case VirtualKey.F9:
					return Key.F9;
				case VirtualKey.F10:
					return Key.F10;
				case VirtualKey.F11:
					return Key.F11;
				case VirtualKey.F12:
					return Key.F12;
				case VirtualKey.NUMLOCK:
					return Key.NumLock;
				case VirtualKey.SCROLL:
					return Key.Scroll;
				case VirtualKey.LSHIFT:
					return Key.LeftShift;
				case VirtualKey.RSHIFT:
					return Key.RightShift;
				case VirtualKey.LCONTROL:
					return Key.LeftCtrl;
				case VirtualKey.RCONTROL:
					return Key.RightCtrl;
				case VirtualKey.LMENU:
					return Key.LeftAlt;
				case VirtualKey.RMENU:
					return Key.RightCtrl;
			}

			return Key.None;
		}

		private static RawInputModifiers GetModifiers(ulong wParam)
		{
			MouseInputKeyStateFlags sourceFlags = (MouseInputKeyStateFlags)wParam;
			RawInputModifiers outModifiers = RawInputModifiers.None;

			if ((sourceFlags & MouseInputKeyStateFlags.MK_CONTROL) != 0)
			{
				outModifiers |= RawInputModifiers.Control;
			}
			if ((sourceFlags & MouseInputKeyStateFlags.MK_LBUTTON) != 0)
			{
				outModifiers |= RawInputModifiers.LeftMouseButton;
			}
			if ((sourceFlags & MouseInputKeyStateFlags.MK_MBUTTON) != 0)
			{
				outModifiers |= RawInputModifiers.MiddleMouseButton;
			}
			if ((sourceFlags & MouseInputKeyStateFlags.MK_RBUTTON) != 0)
			{
				outModifiers |= RawInputModifiers.RightMouseButton;
			}
			if ((sourceFlags & MouseInputKeyStateFlags.MK_SHIFT) != 0)
			{
				outModifiers |= RawInputModifiers.Shift;
			}
			if ((sourceFlags & MouseInputKeyStateFlags.MK_XBUTTON1) != 0)
			{
				outModifiers |= RawInputModifiers.XButton1MouseButton;
			}
			if ((sourceFlags & MouseInputKeyStateFlags.MK_XBUTTON2) != 0)
			{
				outModifiers |= RawInputModifiers.XButton2MouseButton;
			}

			return outModifiers;
		}

		protected unsafe override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
		{
			// Create Hwnd.
			IPlatformHandle platformHandle = base.CreateNativeControlCore(parent);
			Hwnd = platformHandle.Handle;
			
			// Make it clear that the bounds are invalid, and request a new measurement.
			Bounds = new(0, 0, -1, -1);
			InvalidateArrange();

			// Override WndProc.
			baseWndProc = Marshal.GetDelegateForFunctionPointer<WndProc>(User32Methods.GetWindowLongPtr(Hwnd, (int)WindowLongFlags.GWLP_WNDPROC));
			User32Methods.SetWindowLongPtr(Hwnd, (int)WindowLongFlags.GWLP_WNDPROC, Marshal.GetFunctionPointerForDelegate(overrideWndProc));

			hosts.Add(this);

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

			hosts.Remove(this);
			base.DestroyNativeControlCore(control);
		}
		#endregion

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
}