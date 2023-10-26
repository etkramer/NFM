using Avalonia.Input;
using Avalonia.Threading;

namespace NFM;

public enum ButtonState
{
	Up,
	Down
}

public static class Input
{
	public static Vector2 MouseDelta { get; private set; }
	public static object InputSource { get; private set; }

	// Store button states.
	private static Dictionary<MouseButton, ButtonState> mouseStates = new();
	private static Dictionary<Key, ButtonState> keyStates = new();

	// Store pointer states.
	private static bool wasMouseMoved = false;
	private static Vector2 mousePos = Vector2.NaN;
	private static Vector2 lastMousePos = Vector2.NaN;

	static Input()
	{
		DispatcherTimer.Run(() =>
		{
			OnUpdate();
			return true;
		}, TimeSpan.Zero);
	}

	/// <summary>
	/// Checks if the specified key is currently being pressed.
	/// </summary>
	public static bool IsDown(Key key)
	{
		if (keyStates.TryGetValue(key, out ButtonState state))
		{
			return state == ButtonState.Down;
		}

		return false;
	}

	/// <summary>
	/// Checks if the specified key is currently being pressed.
	/// </summary>
	public static bool IsDown(MouseButton button)
	{
		if (mouseStates.TryGetValue(button, out ButtonState state))
		{
			return state == ButtonState.Down;
		}

		return false;
	}

	private static void OnUpdate()
	{
		// No update, therefore no delta.
		if (!wasMouseMoved)
		{
			MouseDelta = Vector2.Zero;
		}
		// No previous position, therefore this is the first update.
		else if (lastMousePos == Vector2.NaN)
		{
			MouseDelta = Vector2.Zero;
		}
		// Nothing fancy here, just update the mouse delta.
		else
		{
			MouseDelta = lastMousePos - mousePos;
		}

		lastMousePos = mousePos;
		wasMouseMoved = false;
	}

	/// <summary>
	/// Updates the input system with a new mouse event.
	/// </summary>
	public static void UpdateMouse(PointerPoint point)
	{
		// Update mouse pointer.
		mousePos = new Vector2((float)point.Position.X, (float)point.Position.Y);
		wasMouseMoved = true;

		// Update mouse buttons.
		mouseStates[MouseButton.Left] = point.Properties.IsLeftButtonPressed ? ButtonState.Down : ButtonState.Up;
		mouseStates[MouseButton.Right] = point.Properties.IsRightButtonPressed ? ButtonState.Down : ButtonState.Up;
		mouseStates[MouseButton.Middle] = point.Properties.IsMiddleButtonPressed ? ButtonState.Down : ButtonState.Up;

		// Update captured input source.
		InputSource = point.Pointer.Captured;
	}

	/// <summary>
	/// Updates the input system with a new keyboard input.
	/// </summary>
	public static void UpdateKey(Key key, bool down)
	{
		keyStates[key] = down ? ButtonState.Down : ButtonState.Up;
	}
}
