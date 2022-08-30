using System;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Engine.Frontend
{
	public class BoolInput : BaseInput
	{
		[Notify] private bool? Value
		{
			get
			{
				return HasMultipleValues ? null : GetFirstValue<bool>();
			}
			set
			{
				if (value.HasValue)
				{
					SetValue(value.Value);
				}
			}
		}

		public BoolInput(PropertyInfo property) : base(property)
		{
			OnSelectedPropertyChanged += () => (this as INotify).Raise(nameof(Value));

			CheckBox checkBox = new CheckBox();
			checkBox.Bind(ToggleButton.IsCheckedProperty, nameof(Value), this);

			Content = checkBox;
		}
	}
}