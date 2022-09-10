using System;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Layout;
using Avalonia.Media;
using Engine.Frontend.Controls;

namespace Engine.Frontend
{
	public partial class LandingDialog : Dialog
	{
		[Save] public static bool ShowOnStartup { get; private set; } = false;

		public LandingDialog()
		{
			DataContext = this;

			Width = 450;
			Height = 650;
			Title = "Landing";

			Content = new Grid()
				.Rows("*, 1, 60")
				.Children(
					new StackPanel()
						.Row(0)
						.Spacing(10)
						.Margin(15)
						.Orientation(Orientation.Vertical)
						.Children(
							new OptionField()
								.Label("Name")
								.Description("Choose a name for your project")
								.Subject("Sandbox"),
							new OptionField()
								.Label("Aspect ratio")
								.Description("Choose the project's aspect ratio")
								.Subject("2.35:1"),
							new OptionField()
								.Label("Embed content")
								.Description("Embed custom content in .project file?")
								.Subject("True")
						),
					new Rectangle()
						.Row(1)
						.Background(this.GetResourceBrush("WindowBackground")),
					new StackPanel()
						.Row(2)
						.Spacing(10)
						.HorizontalAlignment(HorizontalAlignment.Center)
						.VerticalAlignment(VerticalAlignment.Center)
						.Orientation(Orientation.Horizontal)
						.Children(
							new Button()
								.Content("Create")
								.Width(100)
								.Height(26)
								.Style("dialog2")
								.OnClick(CreatePressed),
							new Button()
								.Content("Load")
								.Width(100)
								.Height(26)
								.Style("dialog1")
								.OnClick(LoadPressed)
						)
				);
		}

		public void CreatePressed()
		{
			Project.Create();
			Close();
		}

		public async void LoadPressed()
		{
			string openPath = await ShowOpenDialog(this, new FileFilter("Project file", "project"));

			if (openPath != null)
			{
				Project.Load(openPath);
				Close();
			}
		}
	}
}