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
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace Engine.Frontend
{
	public class VectorInspector : BaseInspector
	{
		[Notify] private string ValueX
		{
			get
			{
				object vec = GetFirstValue<object>();
				return vecIndexer.GetValue(vec, new object[] { 0 }).ToString();
			}
		}

		[Notify] private string ValueY
		{
			get
			{
				object vec = GetFirstValue<object>();
				return vecIndexer.GetValue(vec, new object[] { 1 }).ToString();
			}
		}

		[Notify] private string ValueZ
		{
			get
			{
				object vec = GetFirstValue<object>();
				return vecIndexer.GetValue(vec, new object[] { 2 }).ToString();
			}
		}

		[Notify] private string ValueW
		{
			get
			{
				object vec = GetFirstValue<object>();
				return vecIndexer.GetValue(vec, new object[] { 3 }).ToString();
			}
		}

		private PropertyInfo vecIndexer;

		public VectorInspector(PropertyInfo property, IEnumerable<object> subjects) : base(property, subjects)
		{
			OnSelectedPropertyChanged += () => (this as INotify).Raise(nameof(ValueX));
			OnSelectedPropertyChanged += () => (this as INotify).Raise(nameof(ValueY));
			OnSelectedPropertyChanged += () => (this as INotify).Raise(nameof(ValueZ));
			OnSelectedPropertyChanged += () => (this as INotify).Raise(nameof(ValueW));

			int numComponents = GetComponents(property.PropertyType);
			List<Control> componentInputs = new();

			// Grab the this[] indexer property.
			vecIndexer = property.PropertyType.GetProperty("Item");

			for (int i = 0; i < numComponents; i++)
			{
				int iCaptured = i;

				Control icon = new Panel()
					.Background("#19E6E62E")
					.Width(16)
					.Height(16)
					.Children(new TextBlock()
						.Text(GetIconChar(i).ToString())
						.Size(12)
						.Font(this.GetResource<FontFamily>("IconsFont"))
						.Foreground("#E6E62E")
						.VerticalAlignment(VerticalAlignment.Center)
						.HorizontalAlignment(HorizontalAlignment.Center)
					);

				string componentProp = null;
				switch (i)
				{
					case 0:
						componentProp = nameof(ValueX);
						break;
					case 1:
						componentProp = nameof(ValueY);
						break;
					case 2:
						componentProp = nameof(ValueZ);
						break;
					case 3:
						componentProp = nameof(ValueW);
						break;
				}

				// Create input box.
				TextBox numEntry = new TextBox();
				numEntry.Padding = new(4, 0);
				numEntry.VerticalContentAlignment = VerticalAlignment.Center;
				numEntry.Bind(TextBox.TextProperty, componentProp, this);
				numEntry.Foreground = this.GetResourceBrush("ThemeForegroundMidBrush");
				numEntry.LostFocus += (o, e) => (this as INotify).Raise(componentProp);

				// Ignore alphabetical inputs.
				numEntry.AddHandler(TextInputEvent, (o, e) =>
				{
					if (!e.Text.All(c => !char.IsLetter(c)))
					{
						e.Handled = true;
					}
				},
				RoutingStrategies.Tunnel);

				// Respond to enter key.
				numEntry.KeyDown += (o, e) =>
				{
					if (e.Key == Key.Enter)
					{
						// Set input to new value.
						object vec = GetFirstValue<object>();
						if (TryParseNum(numEntry.Text, typeof(float), out object num))
						{
							vecIndexer.SetValue(vec, num, new object[] { iCaptured });
							SetValue(vec);
						}

						// Switch focus.
						Focus();
					}
				};

				componentInputs.Add(new ContentControl()
					.Margin(i == 0 ? 0 : 4, 0, (i == numComponents - 1 ? 0 : 4), 0)
					.Radius(2)
					.Background(this.GetResourceBrush("ControlBackgroundColor"))
					.With(o => o.Padding = new(1))
					.Content(
						new Grid()
							.Columns("auto, *")
							.Children(icon.Column(0), numEntry.Column(1))
					));
			}

			Content = new UniformGrid()
				.With(o => o.Columns = numComponents)
				.Children(componentInputs.ToArray());
		}

		private int GetComponents(Type type)
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

			return -1;
		}

		private char GetIconChar(int component)
		{
			if (component == 0)
				return 'X';
			else if (component == 1)
				return 'Y';
			else if (component == 2)
				return 'Z';
			else if (component == 3)
				return 'W';

			return default;
		}
	}
}