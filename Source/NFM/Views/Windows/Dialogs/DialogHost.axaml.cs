using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;

namespace NFM.Frontend
{
	public partial class DialogHost : Window
	{
		public static AvaloniaProperty DialogProperty = AvaloniaProperty.Register<DialogHost, Dialog>(nameof(Dialog));
		public Dialog Dialog
		{
			get => (Dialog)GetValue(DialogProperty);
			set => SetValue(DialogProperty, value);
		}

		public DialogHost()
		{
			InitializeComponent();
			DataContext = this;
		}
		
		public DialogHost(Dialog dialog) : this()
		{
			Dialog = dialog;
			SizeToContent = SizeToContent.WidthAndHeight;
		}
	}
}