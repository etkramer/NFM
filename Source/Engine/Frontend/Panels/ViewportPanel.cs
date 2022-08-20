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
	public class ViewportPanel : ToolPanel
	{
		[Notify] string frameTime => $"Frametime: {frameTimeAverager.Result.ToString("0.00")}ms";
		[Notify] string memory => $"Memory: {Environment.WorkingSet / 1024 / 1024}MB";
		private Averager frameTimeAverager = new Averager(100);

		public ViewportPanel()
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
		public Swapchain Swapchain { get; private set; }
		private Viewport viewport;
		private NativeControlHostEx nativeControl;

		public ViewportHost()
		{
			nativeControl = new();
			this.Background("Transparent");
			this.Children(nativeControl);

			// Opened event.
			nativeControl.OnOpen += () =>
			{
				Swapchain = new Swapchain(nativeControl.Hwnd);
				viewport = new Viewport(this);
			};

			// Closed event.
			nativeControl.OnClose += () =>
			{
				viewport.Dispose();
				Swapchain.Dispose();
			};

			// Resized event.
			nativeControl.OnResize += (size) =>
			{
				Swapchain.Resize(size);
			};
		}

		protected override void OnPointerMoved(PointerEventArgs e)
		{
			PointerPointProperties props = e.GetCurrentPoint(this).Properties;

			if (props.IsRightButtonPressed)
			{
				// Capture cursor.
			}
			else
			{
				// Uncapture cursor.
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
}