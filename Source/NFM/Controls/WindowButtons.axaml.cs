using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace NFM;

public class WindowButtons : TemplatedControl
{
	public WindowButtons()
	{
		DataContext =this;
	}

	public void MinimizePressed()
	{
		GetWindow().WindowState = WindowState.Minimized;
	}

	public void MaximizePressed()
	{
		Window window = GetWindow();
		window.WindowState = (window.WindowState == WindowState.Maximized) ? WindowState.Normal : WindowState.Maximized;
	}

	public void ClosePressed()
	{
		GetWindow().Close();
	}

	public event Action DoThing = delegate {};

	private Window GetWindow(IControl top = null)
	{
		if (top == null)
		{
			top = this;
		}

		if (top.Parent == null)
		{
			return null;
		}

		if (top.Parent is Window window)
		{
			return window;
		}
		else
		{
			return GetWindow(top.Parent);
		}
	}
}
