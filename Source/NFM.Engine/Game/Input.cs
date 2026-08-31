using System.Windows.Forms;

namespace NFM;

public enum ButtonState
{
	Up,
	Down
}

public static class Input
{
	public static Vector2 MouseDelta { get; private set; }
	public static object? InputSource { get; private set; }

	/// <summary>
	/// Fired when a key first goes down, with the modifier flags held at that moment.
	/// </summary>
	public static event Action<Keys> OnKeyPressed = delegate {};

	/// <summary>
	/// The modifier flags (<see cref="Keys.Control"/> and friends) for whichever modifiers are held.
	/// </summary>
	public static Keys Modifiers =>
		(IsDown(Keys.LControlKey) || IsDown(Keys.RControlKey) ? Keys.Control : Keys.None) |
		(IsDown(Keys.LShiftKey) || IsDown(Keys.RShiftKey) ? Keys.Shift : Keys.None) |
		(IsDown(Keys.LMenu) || IsDown(Keys.RMenu) ? Keys.Alt : Keys.None);

	// Store button states.
	private static Dictionary<MouseButtons, ButtonState> mouseStates = new();
	private static Dictionary<Keys, ButtonState> keyStates = new();

	// Store pointer states.
	private static bool wasMouseMoved = false;
	private static Vector2 mousePos = Vector2.NaN;
	private static Vector2 lastMousePos = Vector2.NaN;

	/// <summary>
	/// Checks if the specified key is currently being pressed.
	/// </summary>
	public static bool IsDown(Keys key)
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
	public static bool IsDown(MouseButtons button)
	{
		if (mouseStates.TryGetValue(button, out ButtonState state))
		{
			return state == ButtonState.Down;
		}

		return false;
	}

	/// <summary>
	/// Recomputes per-frame input state. Called once per engine tick.
	/// </summary>
	internal static void Update()
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
	/// Updates the input system with a new mouse event. <paramref name="source"/> identifies whichever
	/// host currently owns the pointer, and is null when it belongs to none of them.
	/// </summary>
	public static void UpdateMouse(Vector2 position, bool left, bool right, bool middle, object? source)
	{
		mousePos = position;
		wasMouseMoved = true;

		mouseStates[MouseButtons.Left] = left ? ButtonState.Down : ButtonState.Up;
		mouseStates[MouseButtons.Right] = right ? ButtonState.Down : ButtonState.Up;
		mouseStates[MouseButtons.Middle] = middle ? ButtonState.Down : ButtonState.Up;

		InputSource = source;
	}

	/// <summary>
	/// Updates the input system with a new keyboard input.
	/// </summary>
	public static void UpdateKey(Keys key, bool down)
	{
		bool wasDown = IsDown(key);
		keyStates[key] = down ? ButtonState.Down : ButtonState.Up;

		if (down && !wasDown)
		{
			OnKeyPressed.Invoke(key | Modifiers);
		}
	}

	/// <summary>
	/// Drops every held key. Used when the window loses focus, which would otherwise leave
	/// keys stuck down until they're next pressed.
	/// </summary>
	public static void ReleaseAll()
	{
		keyStates.Clear();
		mouseStates.Clear();
	}
}
