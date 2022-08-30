using System;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Engine.Frontend
{
	public class VectorInput : UserControl
	{
		public VectorInput(IEnumerable<object> subjects, PropertyInfo property)
		{
			const int spacing = 4;

			int numComponents = GetComponents(property.PropertyType);
			List<Control> componentInputs = new();

			// Grab the this[] indexer property.
			PropertyInfo vecIndexer = property.PropertyType.GetProperty("Item");
			
			for (int i = 0; i < numComponents; i++)
			{
				// I have no idea why this helps, but it does.
				int iProxy = i;

				// We don't know the vector's type, so we're going to have to use reflection.
				Func<object> componentGetter = () =>
				{
					// Grab the component's value.
					return vecIndexer.GetValue(property.GetValue(subjects.First()), new object[] {iProxy});
				};
				
				Action<object> componentSetter = (o) =>
				{
					// Modify the vector to use the new component value and set.
					ValueType baseVector = (ValueType)property.GetValue(subjects.First());
					vecIndexer.SetValue(baseVector, Convert.ChangeType(o, GetComponentType(property.PropertyType)), new object[] {iProxy});

					foreach (object subject in subjects)
					{
						property.SetValue(subject, baseVector);
					}
				};

				NumInputLegacy numInput = new NumInputLegacy(componentGetter, componentSetter, subjects.HasVariation((o) => vecIndexer.GetValue(property.GetValue(o), new object[] { iProxy })), GetIconChar(i));
				numInput.Margin = new(i == 0 ? 0 : spacing, 0, i == numComponents - 1 ? 0 : spacing, 0);

				componentInputs.Add(numInput);
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

		private Type GetComponentType(Type vectorType)
		{
			if (vectorType == typeof(Vector2) || vectorType == typeof(Vector3) || vectorType == typeof(Vector4))
				return typeof(float);
			else if (vectorType == typeof(Vector2i) || vectorType == typeof(Vector3i) || vectorType == typeof(Vector4i))
				return typeof(int);
			else if (vectorType == typeof(Vector2d) || vectorType == typeof(Vector3d) || vectorType == typeof(Vector4d))
				return typeof(double);

			return null;
		}
	}
}