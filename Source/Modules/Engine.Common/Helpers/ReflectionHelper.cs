using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Engine.Common
{
	public static class ReflectionHelper
	{
		private static List<Assembly> assemblies = new();
		private static List<MethodInfo> methods = new();
		private static List<FieldInfo> fields = new();
		private static List<PropertyInfo> properties = new();
		private static List<Type> types = new();

		public static void RegisterAssembly(Assembly assembly = null)
		{
			if (assembly == null)
			{
				assembly = Assembly.GetCallingAssembly();
			}

			if (!assemblies.Contains(assembly))
			{
				assemblies.Add(assembly);

				UpdateTypes();
				UpdateProperties();
				UpdateFields();
				UpdateMethods();
			}
		}

		private static void UpdateTypes()
		{
			types.Clear();
			foreach (Assembly assembly in assemblies)
			{
				foreach (Type type in assembly.GetTypes())
				{
					types.Add(type);
				}
			}
		}

		private static void UpdateFields()
		{
			fields.Clear();
			foreach (Type type in types)
			{
				foreach (FieldInfo field in type.GetFields())
				{
					fields.Add(field);
				}
			}
		}

		private static void UpdateProperties()
		{
			properties.Clear();
			foreach (Type type in types)
			{
				foreach (PropertyInfo property in type.GetProperties())
				{
					properties.Add(property);
				}
			}
		}

		private static void UpdateMethods()
		{
			methods.Clear();
			foreach (Type type in types)
			{
				foreach (MethodInfo method in type.GetMethods())
				{
					methods.Add(method);
				}
			}
		}

		private static BindingFlags BindingFlagsAll = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
		private static BindingFlags BindingFlagsAllStatic = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;

		public static MethodInfo[] GetMethodsWithAttribute<T>() where T : Attribute
		{
			List<MethodInfo> attributeMethods = new();
			foreach (MethodInfo method in methods)
			{
				if (method.GetCustomAttribute<T>() != null)
				{
					attributeMethods.Add(method);
				}
			}

			return attributeMethods.ToArray();
		}

		public static bool IsStatic(this Type type)
		{
			return type.IsSealed && type.IsAbstract;
		}

		public static bool HasStaticProperties(this Type type)
		{
			return type.GetProperties(BindingFlagsAllStatic).Length != 0;
		}

		public static bool HasPropertiesWithAttribute<T>(this Type type) where T : Attribute
		{
			foreach (PropertyInfo property in type.GetProperties(BindingFlagsAll))
			{
				if (property.HasAttribute<T>())
				{
					return true;
				}
			}

			return false;
		}

		public static bool HasStaticPropertiesWithAttribute<T>(this Type type) where T : Attribute
		{
			foreach (PropertyInfo property in type.GetProperties(BindingFlagsAllStatic))
			{
				if (property.HasAttribute<T>())
				{
					return true;
				}
			}

			return false;
		}

		public static bool HasAttribute<T>(this PropertyInfo property) where T : Attribute
		{
			return property.GetCustomAttribute<T>() != null;
		}

		public static PropertyInfo[] GetAllProperties(this Type type)
		{
			return type.GetProperties(BindingFlagsAll);
		}

		public static PropertyInfo[] GetAllStaticProperties(this Type type)
		{
			return type.GetProperties(BindingFlagsAllStatic);
		}

		public static PropertyInfo GetPropertyWithAttribute<T>(this Type type, string name) where T : Attribute
		{
			PropertyInfo property = type.GetProperty(name, BindingFlagsAll);
			return property.HasAttribute<T>() ? property : null;
		}

		public static bool InheritsFrom(this Type a, Type b)
		{
			if (a == b)
			{
				return true;
			}

			if (a.BaseType == null)
			{
				return false;
			}

			if (a.BaseType == b)
			{
				return true;
			}
			else
			{
				return InheritsFrom(a.BaseType, b);
			}
		}

		public static Type FindCommonAncestor(IEnumerable<Type> types)
		{
			return FindCommonAncestorRecurse(types, types.First());
		}

		private static Type FindCommonAncestorRecurse(IEnumerable<Type> types, Type baseCandidate)
		{
			if (types.All(o => o.InheritsFrom(baseCandidate)))
			{
				return baseCandidate;
			}
			else if (baseCandidate.BaseType != null)
			{
				return FindCommonAncestorRecurse(types, baseCandidate.BaseType);
			}

			return null;
		}
	}
}
