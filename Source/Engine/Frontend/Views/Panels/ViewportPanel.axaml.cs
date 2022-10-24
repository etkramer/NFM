using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Engine.Editor;
using Engine.GPU;
using Engine.Rendering;
using Engine.World;

namespace Engine.Frontend
{
	public partial class ViewportPanel : ToolPanel
	{
		[Notify] string frameTime => $"Frametime: {frameTimeAverager.Result.ToString("0.00")}ms";
		[Notify] string memory => $"Memory: {Environment.WorkingSet / 1024 / 1024}MB";
		private Averager frameTimeAverager = new Averager(100);

		public ViewportPanel()
		{
			DataContext = this;
			InitializeComponent();

			// Update frametime.
			Game.OnTick += (t) => frameTimeAverager.AddValue(Graphics.FrameTime * 1000);
			Game.OnTick += (t) => (this as INotify).Raise(nameof(memory));
			(frameTimeAverager as INotify).Subscribe(nameof(Averager.Result), () => (this as INotify).Raise(nameof(frameTime)));
		}
	}

	public partial class ViewportHost : Panel
	{
		public Swapchain Swapchain { get; private set; }
		private Viewport viewport;
		private HwndControl nativeControl;

		public ViewportHost()
		{
			nativeControl = new();
			this.Background("Transparent");
			this.Children(nativeControl);

			// Opened event.
			nativeControl.OnOpen += () =>
			{
				Swapchain = new Swapchain(nativeControl.Hwnd, 0);
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

		private void UpdatePointer(PointerEventArgs e)
		{
			// Update input.
			PointerPointProperties props = e.GetCurrentPoint(this).Properties;
			PointerPoint point = e.GetCurrentPoint(null);
			Input.UpdateMouse(point);

			// Capture when held.
			if (props.IsLeftButtonPressed || props.IsRightButtonPressed)
			{
				e.Pointer.Capture(this);
			}
			else
			{
				e.Pointer.Capture(null);
			}
		}

		protected override void OnPointerMoved(PointerEventArgs e)
		{
			UpdatePointer(e);
			base.OnPointerMoved(e);
		}

		protected override void OnPointerPressed(PointerPressedEventArgs e)
		{
			// Make sure to focus this control.
			// It wouldn't happen automatically with right click, which would mean no keyboard input.
			Focus();

			UpdatePointer(e);
			base.OnPointerPressed(e);
		}

		protected override void OnPointerReleased(PointerReleasedEventArgs e)
		{
			UpdatePointer(e);
			base.OnPointerReleased(e);
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			Input.UpdateKey(e.Key, true);
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			Input.UpdateKey(e.Key, false);
		}
	}
}
