using System;
using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using Engine.GPU;
using Engine.Rendering;
using ReactiveUI;

namespace Engine.Frontend
{
	public partial class ViewportPanel : ReactiveToolPanel<ViewportModel>
	{
		public ViewportPanel()
		{
			ViewModel = new ViewportModel();

			this.WhenActivated(d =>
			{

			});

			InitializeComponent();
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