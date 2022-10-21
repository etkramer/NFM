using Avalonia.Controls;

namespace Engine.Frontend
{
	public partial class LandingDialog : Dialog
	{
		[Save] public static bool ShowOnStartup { get; private set; } = false;

		public LandingDialog()
		{
			DataContext = this;
			InitializeComponent();
		}

		public void CreatePressed()
		{
			Project.Reset();
			Close();
		}

		public async void LoadPressed()
		{
			string openPath = await ShowOpenDialog(this, new FileFilter("Project", "json"));

			if (openPath != null)
			{
				Project.Load(openPath);
				Close();
			}
		}
	}
}
