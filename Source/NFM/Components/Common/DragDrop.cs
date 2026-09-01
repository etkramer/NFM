using Microsoft.AspNetCore.Components.Web;

namespace NFM.Components;

/// <summary>
/// App-wide pointer drag state. A source arms a payload on pointer down and a target reads it back on pointer up.
/// The host pumps input by hand into a composition-hosted webview, which rules out HTML5 drag events.
/// </summary>
public static class DragDrop
{
	/// <summary>
	/// Distance the pointer must travel before a press turns into a drag rather than a click.
	/// </summary>
	private const double dragThreshold = 4;

	/// <summary>
	/// Payload of the active drag, or null when nothing is being dragged.
	/// </summary>
	public static object? Payload { get; private set; }

	private static object? pending;
	private static (double X, double Y) pointerStart;

	/// <summary>
	/// Arms a drag that begins once the pointer has travelled far enough.
	/// </summary>
	public static void Arm(object payload, PointerEventArgs args)
	{
		Clear();

		if (args.Button == 0)
		{
			pending = payload;
			pointerStart = (args.ClientX, args.ClientY);
		}
	}

	/// <summary>
	/// Promotes an armed press into a drag, returning the active payload if there is one.
	/// </summary>
	public static object? Update(PointerEventArgs args)
	{
		if (args.Buttons == 0)
		{
			Clear();
			return null;
		}

		if (Payload is null && pending is not null)
		{
			double deltaX = args.ClientX - pointerStart.X;
			double deltaY = args.ClientY - pointerStart.Y;

			if ((deltaX * deltaX) + (deltaY * deltaY) >= dragThreshold * dragThreshold)
			{
				Payload = pending;
			}
		}

		return Payload;
	}

	public static void Clear()
	{
		pending = null;
		Payload = null;
	}
}
