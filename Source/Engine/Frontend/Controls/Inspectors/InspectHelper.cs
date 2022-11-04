using System;
using System.Reflection;
using System.Runtime.Serialization;
using Avalonia.Controls;

namespace Engine.Frontend
{
	public static class InspectHelper
	{
		public static Control Create(object subject, string propertyName, Type typeOverride = null) => Create(new[] { subject }, subject.GetType().GetProperty(propertyName, ReflectionHelper.BindingFlagsAll), typeOverride);
		public static Control Create(object subject, PropertyInfo property, Type typeOverride = null) => Create(new[] { subject}, property, typeOverride);
		public static Control Create(IEnumerable<object> subjects, PropertyInfo property, Type typeOverride = null)
		{
			Type type = typeOverride ?? property.PropertyType;

			foreach (var inspectorType in inspectorTypes)
			{
				var inspectorAttribute = inspectorType.GetCustomAttribute<CustomInspectorAttribute>();

				if (inspectorAttribute.PropertyTypes.Any(o => o.IsAssignableFrom(type)))
				{
					// Create object, *don't* call constructor, and set base properties.
					// This is an *extremely* hacky way to avoid using inheritance.
					var result = FormatterServices.GetUninitializedObject(inspectorType) as Control;
					inspectorType.GetProperty("Property", ReflectionHelper.BindingFlagsAllNonStatic).SetValue(result, property);
					inspectorType.GetProperty("Subjects", ReflectionHelper.BindingFlagsAllNonStatic).SetValue(result, subjects);
					inspectorType.GetProperty("DataContext", ReflectionHelper.BindingFlagsAllNonStatic).SetValue(result, result);

					// Call constructor and return.
					inspectorType.GetConstructor(Type.EmptyTypes).Invoke(result, null);
					return result;
				}
			}
			
			if (type.IsPrimitive)
			{
				// Type is not a composite struct and we're not handling it explicitly. Therefore, it's unsupported.
				return null;
			}
			else
			{
				return null;
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
}