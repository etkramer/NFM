using Avalonia.Controls;

namespace NFM;

public partial class SplashScreen : Window
{
	public SplashScreen()
	{
		InitializeComponent();
		DataContext = this;
	}
}