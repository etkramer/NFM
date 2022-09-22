using System;
using System.Reflection;
using Avalonia;

namespace Engine.Frontend
{
	public class BoolInspector : BaseInspector
	{
		[Notify] private bool? Value
		{
			get
			{
				return GetFirstValue<bool>();
			}
			set
			{
				if (value.HasValue)
				{
					SetValue(value.Value);
				}
			}
		}

		public BoolInspector(PropertyInfo property, IEnumerable<object> subjects) : base(property, subjects)
		{
			OnSelectedPropertyChanged += () => (this as INotify).Raise(nameof(Value));

			// Create and assign bool input.
			Content = new BoolInput();
			(Content as BoolInput).Bind(BoolInput.ValueProperty, nameof(Value), this);
		}
	}
}