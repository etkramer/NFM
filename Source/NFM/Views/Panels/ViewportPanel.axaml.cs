using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using NFM.GPU;
using NFM.Graphics;
using NFM.World;
using ReactiveUI.Reactive;

namespace NFM;

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
	// How far the pointer may travel before a click counts as a drag instead.
	private const double ClickThreshold = 3;

	public Swapchain Swapchain { get; private set; }
	private Viewport viewport;
	private readonly HwndControl nativeControl;

	private Point? pressOrigin;
	private ModelNode pressedNode;

	public ViewportHost()
	{
		nativeControl = new();
		this.Background("Transparent");
		this.Children(nativeControl);

		// Required for the viewport to receive keyboard input.
		Focusable = true;

		// Opened event.
		nativeControl.OnOpen += () =>
		{
			Swapchain = new Swapchain(nativeControl.Hwnd, 0);
			viewport = new Viewport(Swapchain, this);
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

		// Feed the viewport-local position to the renderer, which samples the visbuffer there every frame.
		if (viewport is not null)
		{
			Point local = e.GetPosition(this);
			viewport.CursorPosition = new Vector2i((int)local.X, (int)local.Y);
		}

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

		PointerPointProperties props = e.GetCurrentPoint(this).Properties;
		if (props.IsLeftButtonPressed && !props.IsRightButtonPressed)
		{
			pressOrigin = e.GetPosition(this);

			// What the press landed on, resolved before the pointer has had a chance to move off it.
			pressedNode = viewport?.HoveredNode;
		}
		else
		{
			pressOrigin = null;
		}

		UpdatePointer(e);
		base.OnPointerPressed(e);
	}

	protected override void OnPointerReleased(PointerReleasedEventArgs e)
	{
		if (pressOrigin is Point origin && e.InitialPressMouseButton == MouseButton.Left)
		{
			Point position = e.GetPosition(this);
			bool isDrag = Math.Abs(position.X - origin.X) > ClickThreshold || Math.Abs(position.Y - origin.Y) > ClickThreshold;

			// Alt+Shift+LMB belongs to the camera pan gesture.
			bool isPanning = e.KeyModifiers.HasFlag(KeyModifiers.Alt) && e.KeyModifiers.HasFlag(KeyModifiers.Shift);

			if (!isDrag && !isPanning)
			{
				Select(pressedNode, e.KeyModifiers.HasFlag(KeyModifiers.Control));
			}
		}

		pressOrigin = null;
		pressedNode = null;

		UpdatePointer(e);
		base.OnPointerReleased(e);
	}

	protected override void OnPointerExited(PointerEventArgs e)
	{
		if (viewport is not null)
		{
			viewport.CursorPosition = null;
		}

		base.OnPointerExited(e);
	}

	private static void Select(ModelNode node, bool additive)
	{
		if (!additive)
		{
			Selection.DeselectAll();
		}

		if (node is null)
		{
			return;
		}

		if (Selection.Selected.Contains(node))
		{
			Selection.Deselect(node);
		}
		else
		{
			Selection.Select(node);
		}
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