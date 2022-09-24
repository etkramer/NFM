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
				return GetFirstValue<string>();
			}
			set
			{
				SetValue(value);
			}
		}

		public StringInspector(PropertyInfo property, IEnumerable<object> subjects) : base(property, subjects)
		{
			OnSelectedPropertyChanged += () => (this as INotify).Raise(nameof(Value));

			// Create and assign string input.
			Content = new TextInput();
			(Content as TextInput).Bind(Frontend.TextInput.ValueProperty, nameof(Value), this);
		}
	}
}