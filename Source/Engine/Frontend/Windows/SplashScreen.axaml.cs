using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Media;

namespace Engine.Frontend.Controls
{
	public partial class SplashScreen : Window
	{
		public SplashScreen()
		{
			InitializeComponent();
			DataContext = this;
		}
	}
}