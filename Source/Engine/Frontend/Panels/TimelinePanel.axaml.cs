using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Engine.Editor;
using Engine.World;

namespace Engine.Frontend
{
	public partial class TimelinePanel : ToolPanel
	{
		public TimelinePanel()
		{
			DataContext = this;
			InitializeComponent();
		}
	}
}
