using System;
using System.ComponentModel;
using System.Reflection;
using Avalonia;
using Avalonia.Data;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Engine.Editor;
using System.Collections.Specialized;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace Engine.Frontend
{
	public class VectorInspector : BaseInspector
	{
		[Notify] private object ValueX
		{
			set
			{
				var vector = GetFirstValue<object>();
				vecIndexer.SetValue(vector, value, new object[] {0});
				SetValue(vector);
			}
			get
			{
				return vecIndexer.GetValue(GetFirstValue<object>(), new object[] {0});
			}
		}

		[Notify] private object ValueY
		{
			set
			{
				var vector = GetFirstValue<object>();
				vecIndexer.SetValue(vector, value, new object[] {1});
				SetValue(vector);
			}
			get
			{
				return vecIndexer.GetValue(GetFirstValue<object>(), new object[] {1});
			}
		}

		[Notify] private object ValueZ
		{
			set
			{
				var vector = GetFirstValue<object>();
				vecIndexer.SetValue(vector, value, new object[] {2});
				SetValue(vector);
			}
			get
			{
				return vecIndexer.GetValue(GetFirstValue<object>(), new object[] {2});
			}
		}

		[Notify] private object ValueW
		{
			set
			{
				var vector = GetFirstValue<object>();
				vecIndexer.SetValue(vector, value, new object[] {3});
				SetValue(vector);
			}
			get
			{
				return vecIndexer.GetValue(GetFirstValue<object>(), new object[] {3});
			}
		}

		private PropertyInfo vecIndexer;

		public VectorInspector(PropertyInfo property, IEnumerable<object> subjects) : base(property, subjects)
		{
			// Subscribe to property changed notifications.
			OnSelectedPropertyChanged += () => (this as INotify).Raise(nameof(ValueX));
			OnSelectedPropertyChanged += () => (this as INotify).Raise(nameof(ValueY));
			OnSelectedPropertyChanged += () => (this as INotify).Raise(nameof(ValueZ));
			OnSelectedPropertyChanged += () => (this as INotify).Raise(nameof(ValueW));

			// Count components and grab this[] indexer property.
			int numComponents = GetNumComponents(property.PropertyType);
			vecIndexer = property.PropertyType.GetProperty("Item");

			// Loop through vector components...
			List<Control> inputs = new();
			for (int i = 0; i < numComponents; i++)
			{
				// Create component input.
				NumInput input = new NumInput();
				input.With(o => o.Icon = GetIconChar(i));
				input.With(o => o.IconColor = GetIconForeground(i));
				input.Margin(new Thickness(i == 0 ? 0 : 4, 0, (i == numComponents - 1 ? 0 : 4), 0));
				input.Bind(NumInput.ValueProperty, i switch
				{
					0 => nameof(ValueX),
					1 => nameof(ValueY),
					2 => nameof(ValueZ),
					3 => nameof(ValueW),
					_ => null
				}, this);
				
				inputs.Add(input);
			}

			Content = new UniformGrid()
				.With(o => o.Columns = numComponents)
				.Children(inputs.ToArray());
		}

		private int GetNumComponents(Type type)
		{
			if (type == typeof(Vector2) || type == typeof(Vector2d) || type == typeof(Vector2i))
			{
				return 2;
			}
			else if (type == typeof(Vector3) || type == typeof(Vector3d) || type == typeof(Vector3i))
			{
				return 3;
			}
			else if (type == typeof(Vector4) || type == typeof(Vector4d) || type == typeof(Vector4i))
			{
				return 4;
			}

			return 0;
		}

		private string GetIconChar(int component) => component switch
		{
			0 => "X",
			1 => "Y",
			2 => "Z",
			3 => "W",
			_ => null
		};

		private SolidColorBrush GetIconForeground(int component) => component switch
		{
			0 => SolidColorBrush.Parse("#E26E6E"),
			1 => SolidColorBrush.Parse("#A8CC60"),
			2 => SolidColorBrush.Parse("#84B5E6"),
			3 => SolidColorBrush.Parse("#6E6EE2"),
			_ => null
		};

		private SolidColorBrush GetIconBackground(int component)
		{
			var brush = GetIconForeground(component);
			brush.Opacity = 0.1;
			return brush;
		}
	}
}