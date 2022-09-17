using System;
using System.ComponentModel;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;

namespace Engine.Frontend
{
	public class StringInspector : BaseInspector
	{
		[Notify] private string Value
		{
			get
			{
				return HasMultipleValues ? "--" : GetFirstValue<string>();
			}
			set
			{
				if (value != "--" || !HasMultipleValues)
				{
					SetValue(value);
				}
			}
		}

		public StringInspector(PropertyInfo property, IEnumerable<object> subjects) : base(property, subjects)
		{
			OnSelectedPropertyChanged += () => (this as INotify).Raise(nameof(Value));

			// Create and assign string input.
			Content = new StringInput();
			(Content as StringInput).Bind(StringInput.ValueProperty, nameof(Value), this);
		}
	}
}