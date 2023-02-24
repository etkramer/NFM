using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Avalonia.Layout;
using Avalonia.Data;
using Avalonia.Controls.Shapes;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;

namespace NFM;

public class Dialog
{
	private string title;
	private string message;
	private List<Button> buttons = new();

	private Window win;

	public Dialog(string title, string message)
	{
		this.title = title;
		this.message = message;
	}

	public Dialog Button(string name, Action<Dialog> onClick)
	{
		buttons.Add(
			new Button()
				.Content(name)
				.Width(100)
				.Height(26)
				.Style(buttons.Count == 0 ? "dialog2" : "dialog1")
				.OnClick(() => onClick.Invoke(this)));

		return this;
	}

	public void Show()
	{
		win = new Window();
		win.Title = title;
		win.CanResize = false;
		win.Icon = (Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime).MainWindow.Icon;
		win.SizeToContent = SizeToContent.WidthAndHeight;
		win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
		win.Width = 2; win.Height = 2;

		win.DataContext = win;
		win.Content = new Grid()
			.Rows("auto, *")
			.Children(
				new Grid()
					.Row(0)
					.Columns("auto, *, auto")
					.Children(
						new Image()
							.Column(0)
							.HorizontalAlignment(HorizontalAlignment.Left)
							.Width(24)
							.Margin(10, 2, 0, 0)
							.With(o => o.SetValue(RenderOptions.BitmapInterpolationModeProperty, BitmapInterpolationMode.HighQuality))
							.With(o => o.Source = win.GetResourceBitmap("avares://NFM/Assets/appicon.ico")),
						new TextBlock()
							.Column(1)
							.VerticalAlignment(VerticalAlignment.Center)
							.HorizontalAlignment(HorizontalAlignment.Center)
							.Text(nameof(win.Title), BindingMode.Default)
							.Size(13)
							.Weight(FontWeight.SemiBold)
							.With(o => o.IsHitTestVisible = false)
					)
					.Height(38),
				new Grid()
					.Rows("*, 1, 60")
					.Row(1)
					.Background(win.GetResourceBrush("ThemeControlHighBrush"))
					.Focusable(true)
					.Children(
						new ContentControl()
							.Row(0)
							.Margin(20)
							.Content(
								new TextBlock()
									.HorizontalAlignment(HorizontalAlignment.Center)
									.Text(message)
							),
						new Rectangle()
							.Row(1)
							.Background(win.GetResourceBrush("ThemeControlLowBrush")),				
						new StackPanel()
							.Row(2)
							.Margin(30)
							.Spacing(10)
							.HorizontalAlignment(HorizontalAlignment.Center)
							.VerticalAlignment(VerticalAlignment.Center)
							.Background(win.GetResourceBrush("ThemeControlHighBrush"))
							.Orientation(Orientation.Horizontal)
							.Children(buttons.ToArray())
					)
			);

		// Show the dialog window
		win.ShowDialog((Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime).MainWindow);

		// Auto-select first button
		buttons.FirstOrDefault()?.Focus();
	}

	public void Close()
	{
		win.Close();
	}

	public static async Task<string> ShowSaveDialog(Window parent, params FileFilter[] filters)
	{
		var result = await parent.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions()
		{
			FileTypeChoices = filters.Select(o => new FilePickerFileType(o.Name) { Patterns = o.Extensions.Select(o => $".{o}").ToArray() }).ToArray(),
		});

		if (result == null)
		{
			return null;
		}
		else
		{
			return result.Path.AbsolutePath;
		}
	}

	public static async Task<IEnumerable<string>> ShowOpenDialog(Window parent, params FileFilter[] filters)
	{
		var result = await parent.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
		{
			FileTypeFilter = filters.Select(o => new FilePickerFileType(o.Name) { Patterns = o.Extensions.Select(o => $".{o}").ToArray() }).ToArray(),
		});

		List<string> resultPaths = new();
		foreach (var path in result)
		{
			if (path != null)
			{
				resultPaths.Add(path.Path.AbsolutePath);
			}
		}

		return resultPaths;
	}

	public class FileFilter
	{
		public string Name { get; }
		public IEnumerable<string> Extensions { get; }

		public FileFilter(string name, params string[] extensions)
		{
			Name = name;
			Extensions = extensions;
		}
	}
}
