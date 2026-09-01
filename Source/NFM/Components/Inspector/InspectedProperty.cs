using System.Reflection;

namespace NFM.Components;

/// <summary>
/// One inspected property, shared across however many objects are selected.
/// </summary>
public sealed class InspectedProperty(IReadOnlyList<object> subjects, PropertyInfo property)
{
	public PropertyInfo Property { get; } = property;
	public IReadOnlyList<object> Subjects { get; } = subjects;

	public Type Type => Property.PropertyType;
	public string DisplayName => Property.Name.PascalToDisplay();

	/// <summary>
	/// The value shared by every subject, or null where they disagree.
	/// </summary>
	public object? Value
	{
		get
		{
			object? first = Property.GetValue(Subjects[0]);

			for (int i = 1; i < Subjects.Count; i++)
			{
				if (!Equals(Property.GetValue(Subjects[i]), first))
				{
					return null;
				}
			}

			return first;
		}
		set
		{
			foreach (object subject in Subjects)
			{
				Property.SetValue(subject, value);
			}
		}
	}

	/// <summary>
	/// Reads a single component out of a vector-typed value.
	/// </summary>
	public object? GetComponent(int index) => Value is object vector ? Type.GetProperty("Item")?.GetValue(vector, [index]) : null;

	/// <summary>
	/// Writes a single component back into a vector-typed value.
	/// </summary>
	public void SetComponent(int index, object component)
	{
		object? vector = Value;
		if (vector is null)
		{
			return;
		}

		Type.GetProperty("Item")?.SetValue(vector, component, [index]);
		Value = vector;
	}
}
