﻿using System;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Data;
using System.Linq;
using System.ComponentModel;
using Avalonia.LogicalTree;
using Engine.Resources;
using Engine.Editor;
using ISelectable = Engine.Editor.ISelectable;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections;

namespace Engine.Frontend
{
	public sealed class PropertyInspector : UserControl
	{
		[Notify] public PropertyInfo Property { get; set; }
		[Notify] public IEnumerable<object> Subjects { get; set; }

		[Notify] public Control FieldContent { get; set; }

		public PropertyInspector(IEnumerable<object> subjects, PropertyInfo property)
		{
			Subjects = subjects;
			Property = property;
			DataContext = this;

			Margin = new(0, 4);
			HorizontalAlignment = HorizontalAlignment.Stretch;
			Content = new Grid()
				.Columns("0.5*, *")
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
						.Margin(0, 0, 10, 0)		
						.VerticalAlignment(VerticalAlignment.Center)
						.Content(nameof(FieldContent), BindingMode.Default)
				);

			if (Property.PropertyType == typeof(bool))
			{
				// Boolean input field.
				FieldContent = new BoolInspector(Property, subjects);
			}
			else if (Property.PropertyType == typeof(string))
			{
				// String input field.
				FieldContent = new StringInspector(Property, subjects);
			}
			else if (Property.PropertyType == typeof(sbyte) || Property.PropertyType == typeof(short) || Property.PropertyType == typeof(int) || Property.PropertyType == typeof(long)
				|| Property.PropertyType == typeof(byte) || Property.PropertyType == typeof(ushort) || Property.PropertyType == typeof(uint) || Property.PropertyType == typeof(ulong)
				|| Property.PropertyType == typeof(float) || Property.PropertyType == typeof(double))
			{
				// Numeric input field.
				FieldContent = new NumInspector(Property, subjects);
			}
			else if (Property.PropertyType == typeof(Vector2) || Property.PropertyType == typeof(Vector2i) || Property.PropertyType == typeof(Vector2d)
				|| Property.PropertyType == typeof(Vector3) || Property.PropertyType == typeof(Vector3i) || Property.PropertyType == typeof(Vector3d)
				|| Property.PropertyType == typeof(Vector4) || Property.PropertyType == typeof(Vector4i) || Property.PropertyType == typeof(Vector4d))
			{
				// Vector input field.
				FieldContent = new VectorInspector(Property, subjects);
			}
			else if (Property.PropertyType.IsAssignableTo(typeof(Resource)))
			{
				// Resource reference field.
				FieldContent = new ResourceInspector(Property, subjects);
			}
			else if (Property.PropertyType.IsPrimitive)
			{
				// Type is not a composite struct and we're not handling it explicitly. Therefore, it's unsupported.
			}
			else
			{

			}
		}

	}
}