using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Avalonia.Controls;

namespace NFM;

public static class InspectHelper
{
	public static Control Create(object subject, string propertyName, Type typeOverride = null) => Create(new[] { subject }, subject.GetType().GetProperty(propertyName, ReflectionHelper.BindingFlagsAll), typeOverride);
	public static Control Create(object subject, PropertyInfo property, Type typeOverride = null) => Create(new[] { subject}, property, typeOverride);
	public static Control Create(IEnumerable<object> subjects, PropertyInfo property, Type typeOverride = null)
	{
		var inspectorType = FindInspectorType(typeOverride ?? property.PropertyType);

		if (inspectorType != null)
		{
			// Create object, *don't* call constructor, and set base properties.
			// This is an *extremely* hacky way to avoid using inheritance.
            var res = RuntimeHelpers.GetUninitializedObject(inspectorType) as Control;
			inspectorType.GetProperty("Property", ReflectionHelper.BindingFlagsAllNonStatic).SetValue(res, property);
			inspectorType.GetProperty("Subjects", ReflectionHelper.BindingFlagsAllNonStatic).SetValue(res, subjects);

			// Call constructor and return.
			inspectorType.GetConstructor(Type.EmptyTypes).Invoke(res, null);
			return res;
		}
		else
		{
			return null;
		}
	}

	private static Type FindInspectorType(Type subjectType)
	{
		List<(int, Type)> typeCandidates = new();

		foreach (var inspectorType in inspectorTypes)
		{
			var supportedTypes = inspectorType.GetCustomAttribute<CustomInspectorAttribute>().PropertyTypes;

			// Exact match.
			if (supportedTypes.Any(o => o == subjectType))
			{
				typeCandidates.Add((0, inspectorType));
			}
			// Is descended from this type.
			else if (supportedTypes.Any(o => subjectType.IsAssignableTo(o)))
			{
				typeCandidates.Add((1, inspectorType));
			}
		}

		// Found a match.
		if (typeCandidates.Count > 0)
		{
			return typeCandidates
				.GroupBy(o => o.Item1)
				.LastOrDefault()?
				.Select(o => o.Item2)
				.FirstOrDefault();
		}
		else
		{
			if (subjectType.IsPrimitive)
			{
				// Type is not a composite struct and we're not handling it explicitly. Therefore, it's unsupported.
				return null;
			}
			else
			{
				return null;
			}
		}
	}

	private static List<Type> inspectorTypes = new();
	static InspectHelper()
	{
		foreach (var assembly in ReflectionHelper.LoadedAssemblies)
		{
			foreach (var type in assembly.GetTypes().Where(o => o.HasAttribute<CustomInspectorAttribute>()))
			{
				inspectorTypes.Add(type);
			}
		}
	}
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
public class CustomInspectorAttribute : Attribute
{
	public Type[] PropertyTypes { get; set; }

	public CustomInspectorAttribute(params Type[] types)
	{
		PropertyTypes = types;
	}
}