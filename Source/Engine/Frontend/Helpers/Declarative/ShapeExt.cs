using System;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Data;
using Avalonia.Media;
using Engine.Aspects;

namespace Engine.Frontend
{
	[DeclarativeProperty(SetterName = "Background", PropertyName = "Fill", BindingPropertyName = "FillProperty", PropertyType = typeof(Brush), TargetType = typeof(Shape))]
	public static partial class ShapeExt
	{

	}
}
