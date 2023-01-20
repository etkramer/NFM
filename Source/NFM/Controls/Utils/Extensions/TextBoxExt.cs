using System;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using NFM.Generators;

namespace NFM;

[DeclarativeProperty(SetterName = "Text", PropertyName = "Text", BindingPropertyName = "TextProperty", PropertyType = typeof(string), TargetType = typeof(TextBox))]
[DeclarativeProperty(SetterName = "Hint", PropertyName = "Watermark", BindingPropertyName = "WatermarkProperty", PropertyType = typeof(string), TargetType = typeof(TextBox))]
[DeclarativeProperty(SetterName = "FontFamily", PropertyName = "FontFamily", BindingPropertyName = "FontFamilyProperty", PropertyType = typeof(FontFamily), TargetType = typeof(TextBox))]
[DeclarativeProperty(SetterName = "Foreground", PropertyName = "Foreground", BindingPropertyName = "ForegroundProperty", PropertyType = typeof(Brush), TargetType = typeof(TextBox))]
[DeclarativeProperty(SetterName = "Weight", PropertyName = "FontWeight", BindingPropertyName = "FontWeightProperty", PropertyType = typeof(FontWeight), TargetType = typeof(TextBox))]
public static partial class TextBoxExt
{

}
