﻿using System;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Data;
using System.Linq;

namespace Engine.Frontend
{
	public class PropertyInput : UserControl
	{
		[Notify] public PropertyInfo Property { get; set; }
		[Notify] public IEnumerable<object> Owners { get; set; }

		[Notify] public Control FieldContent { get; set; }

		public PropertyInput(PropertyInfo property, IEnumerable<object> owners)
		{
			Property = property;
			Owners = owners;
			DataContext = this;

			Margin = new(0, 4);
			HorizontalAlignment = HorizontalAlignment.Stretch;
			Content = new Grid()
				.Columns("*, *")
				.Children(
					// Name
					new TextBlock()
						.Column(0)
						.Margin(28, 0, 0, 0)
						.HorizontalAlignment(HorizontalAlignment.Left)
						.VerticalAlignment(VerticalAlignment.Center)
						.Text(Property.Name.PascalToDisplay())
						.Foreground(this.GetResourceBrush("ThemeForegroundMidColor"))
						.Size(11),
					// Field
					new ContentControl()
						.Column(1)
						.Height(20)
						.Margin(0, 0, 10, 0)		
						.VerticalAlignment(VerticalAlignment.Center)
						.Content(nameof(FieldContent), BindingMode.Default)
				);

			CreateField();
		}

		private void CreateField()
		{
			FieldContent = new TextBox()
					.Background(this.GetResourceBrush("ControlBackground"))
					.With(o => o.Padding = new(4, 0))
					.With(o => o.VerticalContentAlignment = VerticalAlignment.Center)
					.Radius(2);
		}
	}
}
