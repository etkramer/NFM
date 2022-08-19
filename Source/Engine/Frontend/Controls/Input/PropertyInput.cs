using System;
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

namespace Engine.Frontend
{
	public class PropertyInput : UserControl
	{
		[Notify] public PropertyInfo Property { get; set; }
		[Notify] public IEnumerable<object> Subjects { get; set; }

		[Notify] public Control FieldContent { get; set; }

		public PropertyInput(IEnumerable<object> subjects, PropertyInfo property)
		{
			Subjects = subjects;
			Property = property;
			DataContext = this;

			Margin = new(0, 4);
			HorizontalAlignment = HorizontalAlignment.Stretch;
			Content = new Grid()
				.Columns("0.8*, *")
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

		protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
		{
			foreach (object subject in Subjects)
			{
				if (subject is INotifyPropertyChanged notifier)
				{
					notifier.PropertyChanged += HandlePropertyChanged;
				}
			}
		}

		protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
		{
			foreach (object subject in Subjects)
			{
				if (subject is INotifyPropertyChanged notifier)
				{
					notifier.PropertyChanged -= HandlePropertyChanged;
				}
			}
		}

		private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == Property.Name)
			{
				CreateField();
			}
		}

		private void CreateField()
		{
			bool hasMultipleValues = Subjects.HasVariation((o) => Property.GetValue(o));

			if (Property.PropertyType == typeof(bool))
			{
				// Boolean input field.
				FieldContent = new BoolInput(() => Property.GetValue(Subjects.First()), (o) => Subjects.ForEach(subject => Property.SetValue(subject, o)), hasMultipleValues);
			}
			else if (Property.PropertyType == typeof(string))
			{
				// String input field.
				FieldContent = new StringInput(() => Property.GetValue(Subjects.First()), (o) => Subjects.ForEach(subject => Property.SetValue(subject, o)), hasMultipleValues);
			}
			else if (Property.PropertyType == typeof(sbyte) || Property.PropertyType == typeof(short) || Property.PropertyType == typeof(int) || Property.PropertyType == typeof(long)
				|| Property.PropertyType == typeof(byte) || Property.PropertyType == typeof(ushort) || Property.PropertyType == typeof(uint) || Property.PropertyType == typeof(ulong)
				|| Property.PropertyType == typeof(float) || Property.PropertyType == typeof(double))
			{
				// Numeric input field.
				FieldContent = new NumInput(() => Property.GetValue(Subjects.First()), (o) => Subjects.ForEach(subject => Property.SetValue(subject, Convert.ChangeType(o, Property.PropertyType))), hasMultipleValues);
			}
			else if (Property.PropertyType.IsAssignableTo(typeof(Resource)))
			{
				// Resource reference field.
				FieldContent = new ResourceInput(() => Property.GetValue(Subjects.First()), (o) => Subjects.ForEach(subject => Property.SetValue(subject, o)), hasMultipleValues);
			}
			else if (Property.PropertyType == typeof(Vector3))
			{
				// Vector input field.
				FieldContent = new VectorInput(() => Property.GetValue(Subjects.First()), (o) => Subjects.ForEach(subject => Property.SetValue(subject, o)), hasMultipleValues, Property);
			}
			else
			{
				FieldContent = new TextBox()
						.Background(this.GetResourceBrush("ControlBackground"))
						.With(o => o.Padding = new(4, 0))
						.With(o => o.VerticalContentAlignment = VerticalAlignment.Center)
						.Radius(2);
			}
		}
	}
}
