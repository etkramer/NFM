using System;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Engine.Frontend
{
	public class VectorInput : UserControl
	{
		public VectorInput(Func<object> getter, Action<object> setter, bool hasMultipleValues, PropertyInfo property)
		{
			const int spacing = 4;

			int numComponents = GetComponents(property.PropertyType);
			List<Control> componentInputs = new();
			
			for (int i = 0; i < numComponents; i++)
			{
				// I have no idea why this helps, but it does.
				int iProxy = i;

				// We don't know the vector's type, so we're going to have to use reflection.
				Func<object> componentGetter = () =>
				{
					// Grab the this[] indexer property.
					PropertyInfo vecIndexer = property.PropertyType.GetProperty("Item");

					// Grab the component's value.
					return vecIndexer.GetValue(getter.Invoke(), new object[] {iProxy});
				};
				
				Action<object> componentSetter = (o) =>
				{
					// Grab the this[] indexer property.
					PropertyInfo vecIndexer = property.PropertyType.GetProperty("Item");

					// Modify the vector to use the new component value and set.
					ValueType baseVector = (ValueType)getter.Invoke();
					vecIndexer.SetValue(baseVector, Convert.ChangeType(o, typeof(float)), new object[] {iProxy});
					setter.Invoke(baseVector);
				};

				NumInput numInput = new NumInput(componentGetter, componentSetter, hasMultipleValues);
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
	}
}