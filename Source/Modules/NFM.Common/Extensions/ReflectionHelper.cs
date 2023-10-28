using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Concurrent;

namespace NFM.Common;

public static class ReflectionHelper
{
	public const BindingFlags BindingFlagsAll = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
	public const BindingFlags BindingFlagsAllStatic = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;
	public const BindingFlags BindingFlagsAllNonStatic = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;

	public static ConcurrentBag<Assembly> LoadedAssemblies { get; } = new();

	static ReflectionHelper()
	{
		RegisterAssembly(Assembly.GetExecutingAssembly());

        var entryAssembly = Assembly.GetEntryAssembly();
        if (entryAssembly is not null)
        {
            RegisterAssembly(entryAssembly);
        }
	}

	public static void RegisterAssembly(Assembly assembly)
	{
		LoadedAssemblies.Add(assembly);
	}

	public static bool IsStatic(this Type type)
	{
		return type.IsSealed && type.IsAbstract;
	}

	public static bool HasStaticProperties(this Type type)
	{
		return type.GetProperties(BindingFlagsAllStatic).Length != 0;
	}

	public static bool TryGetAttribute<T>(this Type type, out T? result) where T : Attribute
	{
		result = type.GetCustomAttribute<T>();
		return result is not null;
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
		return property.GetCustomAttribute<T>() is not null;
	}

	public static bool HasAttribute<T>(this Type type) where T : Attribute
	{
		return type.GetCustomAttribute<T>() is not null;
	}

	public static PropertyInfo[] GetAllProperties(this Type type)
	{
		return type.GetProperties(BindingFlagsAll);
	}

	public static PropertyInfo[] GetAllStaticProperties(this Type type)
	{
		return type.GetProperties(BindingFlagsAllStatic);
	}

	public static bool InheritsFrom(this Type a, Type b)
	{
		if (a == b)
		{
			return true;
		}

		if (a.BaseType is null)
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

	public static Type? FindCommonAncestor(IEnumerable<Type> types)
	{
		return FindCommonAncestorRecurse(types, types.First());
	}

	private static Type? FindCommonAncestorRecurse(IEnumerable<Type> types, Type baseCandidate)
	{
		if (types.All(o => o.InheritsFrom(baseCandidate)))
		{
			return baseCandidate;
		}
		else if (baseCandidate.BaseType is not null)
		{
			return FindCommonAncestorRecurse(types, baseCandidate.BaseType);
		}

		return null;
	}

	public static Type? GetElementType(this System.Collections.IEnumerable enumerable)
	{
		if (enumerable is Array)
		{
			return enumerable.GetType().GetElementType();
		}
		else
		{
			// Blatant dynamic abuse...
			try
			{
				return GetElementTypeDynamic((dynamic)enumerable);
			}
			catch
			{
				return null;
			}
		}
	}

	private static Type GetElementTypeDynamic<T>(IEnumerable<T> enumerable)
	{
		return typeof(T);
	}
}
