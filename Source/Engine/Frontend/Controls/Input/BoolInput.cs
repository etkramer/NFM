using System;
using System.Reflection;
using Avalonia.Controls;

namespace Engine.Frontend
{
	public class BoolInput : UserControl
	{
		public BoolInput(Func<object> getter, Action<object> setter, bool hasMultipleValues)
		{
			CheckBox checkBox = new CheckBox();
			checkBox.IsChecked = hasMultipleValues ? null : (bool)getter.Invoke();

			checkBox.Checked += (o, e) =>
			{
				setter.Invoke(true);
			};
			checkBox.Unchecked += (o, e) =>
			{
				setter.Invoke(false);
			};

			Content = checkBox;
		}
	}
}