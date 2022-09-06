using System;
using System.ComponentModel;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Engine.Editor;
using Avalonia.Layout;
using System.Data;
using Avalonia.Input.Platform;

namespace Engine.Frontend
{
	public class BaseInput : UserControl, INotify
	{
		public string PropertyName => Property.Name.PascalToDisplay();

		protected PropertyInfo Property { get; private set; }
		protected IEnumerable<object> Subjects { get; private set; }

		protected event Action OnSelectedPropertyChanged = delegate {};
		protected bool HasMultipleValues => Subjects.HasVariation(o => Property.GetValue(o));

		public BaseInput(PropertyInfo property, IEnumerable<object> subjects)
		{
			Property = property;
			Subjects = subjects;
			DataContext = this;

			handlers = new PropertyChangedEventHandler[Subjects.Count()];
		}

		private PropertyChangedEventHandler[] handlers;

		protected sealed override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
		{
			for (int i = 0; i < Subjects.Count(); i++)
			{
				handlers[i] = (o, e) => OnSelectedPropertyChanged.Invoke();

				if (Subjects.ElementAt(i) is INotify notify)
				{
					notify.PropertyChanged += handlers[i];
				}
			}

			base.OnAttachedToVisualTree(e);
		}

		protected sealed override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
		{
			for (int i = 0; i < Subjects.Count(); i++)
			{
				if (Subjects.ElementAt(i) is INotify notify)
				{
					notify.PropertyChanged -= handlers[i];
				}
			}

			base.OnDetachedFromVisualTree(e);
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
			foreach (var obj in Subjects)
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

		private static DataTable computeTable = new();

		protected bool TryParseNum(string text, Type numType, out object num)
		{
			// Interpret emptied field as zero.
			if (string.IsNullOrWhiteSpace(text))
			{
				num = Convert.ChangeType(0, numType);
				return true;
			}

			// Try to evaluate the text as an expression.
			try
			{
				object computedValue = computeTable.Compute(text, null);
				text = computedValue.ToString();
			}
			catch {} // No problem, probably just not a valid expression.

			if (IsFloat(numType))
			{
				if (double.TryParse(text, out double floatValue))
				{
					num = Convert.ChangeType(floatValue, numType);
					return true;
				}
			}
			else
			{
				if (ulong.TryParse(text, out ulong uintValue))
				{
					num = Convert.ChangeType(uintValue, numType);
					return true;
				}
				else if (long.TryParse(text, out long intValue))
				{
					num = Convert.ChangeType(intValue, numType);
					return true;
				}
			}

			num = null;
			return false;
		}

		protected T GetFirstValue<T>()
		{
			return (T)Property.GetValue(Subjects.First());
		}
	}
}