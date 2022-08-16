using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Linq;

namespace Engine.Core
{
	public static class Metadata
	{
		private static ConditionalWeakTable<object, List<(string, Dictionary<Type, object>)>> table = new();

		public static T Store<T>(object subject, string propertyName = null) where T : class, new()
		{
			T metadata = new T();
			Store(subject, metadata, propertyName);
			return metadata;
		}

		public static T Store<T>(object subject, T metadata, string propertyName = null) where T : class
		{
			// Does the object have a property table?
			if (!table.TryGetValue(subject, out var propertyTable))
			{
				propertyTable = new();
				table.Add(subject, propertyTable);
			}

			// Try to find the type table.
			Dictionary<Type, object> typeTable = null;
			foreach (var entry in propertyTable)
			{
				if (entry.Item1 == propertyName)
				{
					typeTable = entry.Item2;
				}
			}

			// Does the object/property have a type table?
			if (typeTable == null)
			{
				typeTable = new();
				propertyTable.Add(new(propertyName, typeTable));
			}

			// Does the object/property already have metadata of this type?
			if (!typeTable.ContainsKey(typeof(T)))
			{
				// If so, add it.
				typeTable.Add(typeof(T), metadata);
			}
			else
			{
				// Otherwise, don't - we don't want more than one instance of each metadata type.
				return metadata;
			}

			return metadata;
		}

		public static T Fetch<T>(object subject, string propertyName = null) where T : class
		{
			if (table.TryGetValue(subject, out var propertyTable))
			{
				foreach (var entry in propertyTable)
				{
					if (entry.Item1 == propertyName)
					{
						if (entry.Item2.TryGetValue(typeof(T), out object metadata))
						{
							return (T)metadata;
						}

						break;
					}
				}
			}

			return null;
		}

		public static bool TryFetch<T>(object subject, out T result, string propertyName = null) where T : class
		{
			result = Fetch<T>(subject, propertyName);

			if (result == null)
			{
				return false;
			}

			return true;
		}
	}
}
