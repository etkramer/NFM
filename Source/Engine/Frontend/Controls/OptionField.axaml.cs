using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Engine.Aspects;

namespace Engine.Frontend.Controls
{
	[DeclarativeProperty(SetterName = "Label", PropertyName = "Label", BindingPropertyName = "LabelProperty", PropertyType = typeof(string), TargetType = typeof(OptionField))]
	[DeclarativeProperty(SetterName = "Description", PropertyName = "Description", BindingPropertyName = "DescriptionProperty", PropertyType = typeof(string), TargetType = typeof(OptionField))]
	[DeclarativeProperty(SetterName = "Subject", PropertyName = "Subject", BindingPropertyName = "SubjectProperty", PropertyType = typeof(object), TargetType = typeof(OptionField))]
	public partial class OptionField : TemplatedControl
	{
		public static readonly AvaloniaProperty<string> LabelProperty = AvaloniaProperty.Register<OptionField, string>(nameof(Label));
		public static readonly AvaloniaProperty<string> DescriptionProperty = AvaloniaProperty.Register<OptionField, string>(nameof(Description));
		public static readonly AvaloniaProperty<object> SubjectProperty = AvaloniaProperty.Register<OptionField, object>(nameof(Subject), "25");

		public string Label
		{
			get => (string)GetValue(LabelProperty);
			set => SetValue(LabelProperty, value);
		}

		public string Description
		{
			get => (string)GetValue(DescriptionProperty);
			set => SetValue(DescriptionProperty, value);
		}

		public object Subject
		{
			get => GetValue(SubjectProperty);
			set => SetValue(SubjectProperty, value);
		}

		public OptionField()
		{
			DataContext = this;
		}
	}
}
