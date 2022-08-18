using System;
using System.Reflection;
using Avalonia.Controls;

namespace Engine.Frontend
{
	public class BoolInput : UserControl
	{
		public BoolInput(IEnumerable<object> subjects, PropertyInfo property)
		{
			bool hasMultipleValues = subjects.HasVariation((o) => property.GetValue(o));

			CheckBox checkBox = new CheckBox();
			checkBox.IsChecked = hasMultipleValues ? null : (bool)property.GetValue(subjects.First());

			checkBox.Checked += (o, e) =>
			{
				foreach (object subject in subjects)
				{
					property.SetValue(subject, true);
				}
			};
			checkBox.Unchecked += (o, e) =>
			{
				foreach (object subject in subjects)
				{
					property.SetValue(subject, false);
				}
			};

			Content = checkBox;
		}
	}
}