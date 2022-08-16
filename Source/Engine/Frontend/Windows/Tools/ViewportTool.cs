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
					new ViewportHost()
						.Row(0),
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

	public unsafe class ViewportHost : NativeControlHost
	{
		public event Action<Vector2i> OnResize = delegate {};

		public IntPtr Hwnd { get; private set; }
		public Swapchain Swapchain { get; private set; }

		private Viewport viewport;

		#region Win32
		// Override WndProc to handle input events.
		private static WndProc baseWndProc;
		private static WndProc overrideWndProc = WndProcOverride;
		private static unsafe IntPtr WndProcOverride(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam)
		{
			switch ((WM)msg)
			{
				case WM.LBUTTONDOWN:
					Debug.Log("Mouse down");
					break;

				default:
					break;
			};

			return baseWndProc(hwnd, msg, wParam, lParam);
		}

		protected unsafe override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
		{
			// Create Hwnd.
			IPlatformHandle platformHandle = base.CreateNativeControlCore(parent);
			Hwnd = platformHandle.Handle;
			InvalidateArrange();

			// Override WndProc.
			baseWndProc = Marshal.GetDelegateForFunctionPointer<WndProc>(User32Methods.GetWindowLongPtr(Hwnd, (int)WindowLongFlags.GWLP_WNDPROC));
			User32Methods.SetWindowLongPtr(Hwnd, (int)WindowLongFlags.GWLP_WNDPROC, Marshal.GetFunctionPointerForDelegate(overrideWndProc));

			return platformHandle;
		}

		protected override void DestroyNativeControlCore(IPlatformHandle control)
		{
			// Cleanup viewport and swapchain.
			viewport.Dispose();
			Swapchain.Dispose();

			base.DestroyNativeControlCore(control);
		}
		#endregion

		protected override Size ArrangeOverride(Size finalSize)
		{
			Size arrangeResult = base.ArrangeOverride(finalSize);
			Vector2i size = new((int)arrangeResult.Width, (int)arrangeResult.Height);

			if (Swapchain == null && viewport == null)
			{
				// The correct size isn't available yet when CreateNativeControlCore is called.
				// Because we need that size for swapchain creation, we wait until ArrangeOverride instead.
				Swapchain = new Swapchain(Hwnd, size);
				viewport = new Viewport(this);
			}
			else if (size != Swapchain.Size)
			{
				// Resize the swapchain if needed.
				Swapchain.Resize(size);
				OnResize.Invoke(size);
			}

			return arrangeResult;
		}
	}
}