using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;

namespace Engine.Frontend
{
	public enum KeyState
	{
		Up,
		Down
	}

	public static class Input
	{
		public static Vector2 MouseDelta { get; private set; }
		public static Control InputSource { get; private set; }

		// Mouse buttons
		public static KeyState LeftMouseButton;
		public static KeyState RightMouseButton;

		// Keyboard buttons
		public static KeyState W;
		public static KeyState A;
		public static KeyState S;
		public static KeyState D;
		public static KeyState Q;
		public static KeyState E;
		public static KeyState C;
		public static KeyState Space;
		public static KeyState Shift;

		private static bool wasMouseMoved = false;
		private static Vector2 mousePos = Vector2.NaN;
		private static Vector2 lastMousePos = Vector2.NaN;

		static Input()
		{
			DispatcherTimer.Run(() =>
			{
				OnTick();
				return true;
			}, TimeSpan.Zero);
		}

		private static void OnTick()
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

		public static void UpdateMouse(PointerPoint point)
		{
			mousePos = new Vector2((float)point.Position.X, (float)point.Position.Y);
			wasMouseMoved = true;

			LeftMouseButton = point.Properties.IsLeftButtonPressed ? KeyState.Down : KeyState.Up;
			RightMouseButton = point.Properties.IsRightButtonPressed ? KeyState.Down : KeyState.Up;

			InputSource = point.Pointer.Captured as Control;
		}

		public static void UpdateKey(Key key, bool down)
		{
			if (key == Key.W)
			{
				W = down ? KeyState.Down : KeyState.Up;
			}
			if (key == Key.A)
			{
				A = down ? KeyState.Down : KeyState.Up;
			}
			if (key == Key.S)
			{
				S = down ? KeyState.Down : KeyState.Up;
			}
			if (key == Key.D)
			{
				D = down ? KeyState.Down : KeyState.Up;
			}
			if (key == Key.Q)
			{
				Q = down ? KeyState.Down : KeyState.Up;
			}
			if (key == Key.E)
			{
				E = down ? KeyState.Down : KeyState.Up;
			}
			if (key == Key.C)
			{
				C = down ? KeyState.Down : KeyState.Up;
			}
			if (key == Key.Space)
			{
				Space = down ? KeyState.Down : KeyState.Up;
			}
			if (key == Key.LeftShift)
			{
				Shift = down ? KeyState.Down : KeyState.Up;
			}
		}
	}
}
