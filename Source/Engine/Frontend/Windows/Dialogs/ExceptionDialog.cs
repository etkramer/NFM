using System;
using Avalonia.Layout;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Engine.Frontend.Controls;
using Avalonia.Data;
using System.Runtime.ExceptionServices;
using Avalonia.Styling;

namespace Engine.Frontend
{
	public class ExceptionDialog : Dialog
	{
		public ExceptionDialog(ExceptionDispatchInfo dispatchInfo = null)
		{
			Exception exception = dispatchInfo.SourceException;
			DataContext = this;
			Title = exception.GetType().Name;
			Content = new Grid()
				.Rows("*, 1, 60")
				.Children(
					new StackPanel()
						.Row(0)
						.Margin(20)
						.Children(
							new TextBlock()
								.Text("An unhandled exception has occured. If you wish to debug this event further, select Break. Otherwise, select Abort to end the program."),
							new TextBlock()
								.Text($"{exception.GetType().Name}: {exception.Message}"),
							new TextBlock()
								.Text($"{exception.StackTrace}")
						),
					new Rectangle()
						.Row(1)
						.Background(this.GetResourceBrush("WindowBackground")),
					new StackPanel()
						.Row(2)
						.Orientation(Orientation.Horizontal)
						.HorizontalAlignment(HorizontalAlignment.Center)
						.VerticalAlignment(VerticalAlignment.Center)
						.Spacing(10)
						.Children(
							new Button()
								.Content("Break")
								.Width(100)
								.Height(26)
								.Style("dialog1")
								.OnClick(() => dispatchInfo.Throw()),
							new Button()
								.Content("Abort")
								.Width(100)
								.Height(26)
								.Style("dialog2")
								.OnClick(() => Environment.Exit(-1))	
						)
				);
		}
	}
}