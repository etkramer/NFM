using System;
using System.ComponentModel;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Engine.Resources;

namespace Engine.Frontend
{
	public class ResourceInspector : BaseInspector
	{
		private Resource Value
		{
			get => HasMultipleValues ? null : GetFirstValue<Resource>();
			set => SetValue(value);
		}

		public ResourceInspector(PropertyInfo property, IEnumerable<object> subjects) : base(property, subjects)
		{
			OnSelectedPropertyChanged += () => (this as INotify).Raise(nameof(Value));

			// Create and assign bool input.
			Content = new ResourceInput();
			(Content as ResourceInput).Bind(ResourceInput.ValueProperty, nameof(Value), this);
		}
	}
}