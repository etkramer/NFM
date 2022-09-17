using System;
using System.ComponentModel;
using System.Reflection;
using Avalonia.Data;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Engine.Editor;
using System.Collections.Specialized;
using Avalonia.Interactivity;

namespace Engine.Frontend
{
	public class NumInspector : BaseInspector
	{
		[Notify] private object Value
		{
			get => HasMultipleValues ? null : GetFirstValue<object>();
			set => SetValue(value);
		}

		public NumInspector(PropertyInfo property, IEnumerable<object> subjects) : base(property, subjects)
		{
			OnSelectedPropertyChanged += () => (this as INotify).Raise(nameof(Value));

			// Create and assign string input.
			Content = new NumInput();
			(Content as NumInput).NumType = GetFirstValue<object>().GetType();
			(Content as NumInput).Bind(NumInput.ValueProperty, nameof(Value), this);
		}
	}
}