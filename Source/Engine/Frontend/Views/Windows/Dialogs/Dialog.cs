using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.LogicalTree;

namespace Engine.Frontend
{
	public abstract class Dialog : UserControl
	{
		public static readonly StyledProperty<string> TitleProperty = AvaloniaProperty.Register<ToolPanel, string>(nameof(Title), "Dialog Window");
		public string Title
		{
			get { return GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}

		private DialogHost host;

		public void Show()
		{
			Focusable = true;
			host = new DialogHost(this);
			host.ShowDialog((Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime).MainWindow);
		}

		public void Close()
		{
			host.Close();
		}

		public static implicit operator DialogHost(Dialog dialog)
		{
			return dialog.host;
		}

		public static async Task<string> ShowSaveDialog(Window parent, params FileFilter[] filters)
		{
			SaveFileDialog saveDialog = new SaveFileDialog()
			{
				Filters = new List<FileDialogFilter>(filters)
			};

			return await saveDialog.ShowAsync(parent);
		}

		public static async Task<string> ShowOpenDialog(Window parent, params FileFilter[] filters)
		{
			OpenFileDialog openDialog = new OpenFileDialog()
			{
				AllowMultiple = false,
				Filters = new List<FileDialogFilter>(filters)
			};

			string[] result = await openDialog.ShowAsync(parent);
			return result?.Length > 0 ? result[0] : null;
		}
	}

	public class FileFilter : FileDialogFilter
	{
		public FileFilter(string name, params string[] extensions)
		{
			Name = name;
			Extensions = new(extensions);
		}
	}
}
