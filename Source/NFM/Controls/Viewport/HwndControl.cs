using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using Avalonia.Input;

namespace NFM;

public unsafe class HwndControl : NativeControlHost
{
	private static List<HwndControl> hosts = new();

	public event Action<Vector2i> OnResize = delegate{};
	public event Action OnOpen = delegate{};
	public event Action OnClose = delegate{};

	public IntPtr Hwnd { get; private set; }
	private bool hasValidMeasure = false;

	protected unsafe override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
	{
		// Create Hwnd.
		IPlatformHandle platformHandle = base.CreateNativeControlCore(parent);
		Hwnd = platformHandle.Handle;
		
		// Make it clear that the bounds are invalid, and request a new measurement.
		Bounds = new(0, 0, -1, -1);
		InvalidateArrange();

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
