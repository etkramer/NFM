using System;
using Avalonia.Data;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Media;
using Avalonia.Layout;
using Engine.Editor;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using ISelectable = Engine.Editor.ISelectable;
using System.Text.RegularExpressions;

namespace Engine.Frontend
{
	public class TimelinePanel : ToolPanel
	{
		public TimelinePanel()
		{
			DataContext = this;

			Title = "Timeline";
			Content = new Grid()
				.Rows("32, 50, *")
				.Background(this.GetResourceBrush("ToolBackgroundColor"));
		}
	}
}