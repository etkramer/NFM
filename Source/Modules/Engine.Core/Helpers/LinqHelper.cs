using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using System.Collections;

namespace Engine.Core
{
	public static class LinqHelper
	{
		private class BucketDict<TKey, TValue>
		{
			public List<(TKey, int)> Keys = new();
			public List<TValue> Values = new();

			public void Add(TKey key, TValue value)
			{
				Values.Add(value);
				Keys.Add(new(key, Values.Count - 1));
			}

			public bool TryGetValue(TKey key, out TValue result)
			{
				for (int i = 0; i < Keys.Count; i++)
				{
					if (Keys[i].Item1 == null && key == null)
					{
						result = Values[i];
						return true;
					}
					else if (Keys[i].Item1 == null)
					{
						result = default;
						return false;
					}
					else if (Keys[i].Item1.Equals(key))
					{
						result = Values[i];
						return true;
					}
				}

				result = default;
				return false;
			}
		}

		/// <summary>
		/// Groups objects that share a certain value into buckets.
		/// </summary>
		public static IEnumerable<IEnumerable<T>> Bucket<T, T2>(this IEnumerable<T> source, Func<T, T2> getter)
		{
			BucketDict<T2, List<T>> bucketDict = new();

			foreach (T subject in source)
			{
				T2 value = getter.Invoke(subject);

				if (!bucketDict.TryGetValue(value, out var bucket))
				{
					bucket = new();
					bucketDict.Add(value, bucket);
				}

				bucket.Add(subject);
			}

			return bucketDict.Values;
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

		public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
		{
			foreach (T value in source)
			{
				action.Invoke(value);
			}
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
