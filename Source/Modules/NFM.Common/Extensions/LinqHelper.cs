using System;
using System.Collections.Generic;
using System.Linq;

namespace NFM.Common
{
	public static class LinqHelper
	{
		/// <inheritdoc cref="List{T}.ForEach"/>
		public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
		{
			foreach (T element in source)
			{
				action.Invoke(element);
			}
		}

		/// <summary>
		/// Checks if there are any objects that don't share a value.
		/// </summary>
		public static bool HasVariation<T, T2>(this IEnumerable<T> source, Func<T, T2> getter)
		{
			T2 lastResult = getter(source.First());

			foreach (T value in source)
			{
				T2 result = getter.Invoke(value);

				if (!result.Equals(lastResult))
				{
					return true;
				}
				else
				{
					lastResult = result;
				}
			}

			return false;
		}

		public static bool TryFirst<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, out TSource value)
		{
			foreach (var item in source)
			{
				if (predicate.Invoke(item))
				{
					value = item;
					return true;
				}
			}

			value = default(TSource);
			return false;
		}
	}
}
