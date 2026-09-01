using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace NFM.Components;

public enum SplitOrientation
{
	Horizontal,
	Vertical
}

public sealed partial class SplitView
{
	private ElementReference element;

	private int size;
	private bool isDragging;
	private double origin;
	private int originSize;

	private bool IsVertical => Orientation is SplitOrientation.Vertical;

	public SplitView()
	{
		ShouldRenderOnEvent = true;
	}

	protected override void OnInitialized() => size = Math.Clamp(InitialSize, Min, Max);

	private double PositionOf(PointerEventArgs args) => IsVertical ? args.ClientY : args.ClientX;

	private async Task OnPointerDown(PointerEventArgs args)
	{
		if (args.Button != 0)
		{
			return;
		}

		isDragging = true;
		origin = PositionOf(args);
		originSize = size;

		await JS.InvokeVoidAsync("HTMLElement.setPointerCapture", element, args.PointerId);
		await JS.InvokeVoidAsync("setBodyCursor", IsVertical ? "ns-resize" : "ew-resize");
	}

	// Capture keeps these arriving while the pointer is outside the handle, so the drag can't be lost.
	private void OnPointerMove(PointerEventArgs args)
	{
		if (isDragging)
		{
			size = Math.Clamp(originSize + (int)(PositionOf(args) - origin), Min, Max);
		}
	}

	private async Task OnPointerUp(PointerEventArgs args)
	{
		if (isDragging)
		{
			await JS.InvokeVoidAsync("HTMLElement.releasePointerCapture", element, args.PointerId);
		}

		await EndDrag();
	}

	private Task OnLostCapture() => EndDrag();

	private async Task EndDrag()
	{
		if (!isDragging)
		{
			return;
		}

		isDragging = false;
		await JS.InvokeVoidAsync("setBodyCursor", "");
	}
}
