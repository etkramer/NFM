using System;
using System.ComponentModel;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Engine.Editor;

namespace Engine.Frontend
{
	public class BaseInput : UserControl, INotify
	{
		protected PropertyInfo Property { get; private set; }
		protected event Action OnSelectedPropertyChanged = delegate {};
		protected bool HasMultipleValues => Selection.Selected.HasVariation(o => Property.GetValue(o));

		public BaseInput(PropertyInfo property)
		{
			Property = property;
			DataContext = this;
		}

		private PropertyChangedEventHandler[] handlers = new PropertyChangedEventHandler[Selection.Selected.Count];

		protected sealed override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
		{
			for (int i = 0; i < Selection.Selected.Count; i++)
			{
				handlers[i] = (o, e) => OnSelectedPropertyChanged.Invoke();

				if (Selection.Selected[i] is INotify notify)
				{
					notify.PropertyChanged += handlers[i];
				}
			}

			base.OnAttachedToVisualTree(e);
		}

		protected sealed override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
		{
			for (int i = 0; i < Selection.Selected.Count; i++)
			{
				if (Selection.Selected[i] is INotify notify)
				{
					notify.PropertyChanged -= handlers[i];
				}
			}

			base.OnDetachedFromVisualTree(e);
		}

		protected bool IsKeyNumeric(Key key)
		{
			return key == Key.D0
				|| key == Key.D1
				|| key == Key.D2
				|| key == Key.D3
				|| key == Key.D4
				|| key == Key.D5
				|| key == Key.D6
				|| key == Key.D7
				|| key == Key.D8
				|| key == Key.D9
				|| key == Key.OemMinus
				|| key == Key.OemPeriod
				|| key == Key.Back
				|| key == Key.Enter;
		}

		protected bool IsUnsigned(Type type)
		{
			return type == typeof(byte)
				|| type == typeof(ushort)
				|| type == typeof(uint)
				|| type == typeof(ulong);
		}

		protected bool IsFloat(Type type)
		{
			return type == typeof(float)
				|| type == typeof(double);
		}

		protected void SetValue<T>(T value)
		{
			foreach (var obj in Selection.Selected)
			{
				object originalVal = Property.GetValue(obj);

				Command.DoCommand(() =>
				{
					Property.SetValue(obj, Convert.ChangeType(value, Property.PropertyType));
				},
				() =>
				{
					Property.SetValue(obj, originalVal);
				}, $"Set value to {value}");
			}
		}

		protected T GetFirstValue<T>()
		{
			return (T)Property.GetValue(Selection.Selected[0]);
		}
	}
}