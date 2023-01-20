using System;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using NFM.Generators;

namespace NFM;

[DeclarativeProperty(SetterName = "Text", PropertyName = "Text", BindingPropertyName = "TextProperty", PropertyType = typeof(string), TargetType = typeof(TextBlock))]
[DeclarativeProperty(SetterName = "Font", PropertyName = "FontFamily", BindingPropertyName = "FontFamilyProperty", PropertyType = typeof(FontFamily), TargetType = typeof(TextBlock))]
[DeclarativeProperty(SetterName = "Size", PropertyName = "FontSize", BindingPropertyName = "FontSizeProperty", PropertyType = typeof(int), TargetType = typeof(TextBlock))]
[DeclarativeProperty(SetterName = "Foreground", PropertyName = "Foreground", BindingPropertyName = "ForegroundProperty", PropertyType = typeof(Brush), TargetType = typeof(TextBlock))]
[DeclarativeProperty(SetterName = "Weight", PropertyName = "FontWeight", BindingPropertyName = "FontWeightProperty", PropertyType = typeof(FontWeight), TargetType = typeof(TextBlock))]
public static partial class TextBlockExt
{

}
