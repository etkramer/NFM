using System;
using Avalonia.Controls;
using Engine.Resources;	

namespace Engine.Frontend
{
	public partial class LibraryPanel : ToolPanel
	{
		public LibraryPanel()
		{
			InitializeComponent();
			DataContext = this;
		}
	}
}