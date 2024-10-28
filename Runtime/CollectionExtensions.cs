using System;
using System.Collections.Generic;

namespace Kryz.SharpUtils
{
	public static class CollectionExtensions
	{
		public static bool TryEnsureCapacity(int currentCapacity, int desiredCapacity, out int newCapacity)
		{
			if (desiredCapacity > currentCapacity)
			{
				newCapacity = (int)Math.Min(int.MaxValue, (uint)currentCapacity * 2);
				newCapacity = Math.Max(newCapacity, desiredCapacity);
				return true;
			}
			newCapacity = currentCapacity;
			return false;
		}

		public static void EnsureCapacity<T>(this List<T> list, int capacity)
		{
			if (TryEnsureCapacity(list.Capacity, capacity, out int newCapacity))
			{
				list.Capacity = newCapacity;
			}
		}

		public static void AddRangeNonAlloc<T>(this List<T> list, IReadOnlyList<T> toAdd)
		{
			int count = toAdd.Count;
			list.EnsureCapacity(list.Count + count);

			for (int i = 0; i < count; i++)
			{
				list.Add(toAdd[i]);
			}
		}

		public static void AddRangeWhere<T>(this List<T> list, IReadOnlyList<T> toAdd, Func<T, bool> predicate)
		{
			int count = toAdd.Count;
			list.EnsureCapacity(list.Count + count);

			for (int i = 0; i < count; i++)
			{
				T item = toAdd[i];
				if (predicate(item))
				{
					list.Add(item);
				}
			}
		}

		public static int IndexOf<T>(this IReadOnlyList<T> list, T value, IEqualityComparer<T>? comparer = null)
		{
			comparer ??= EqualityComparer<T>.Default;

			for (int i = 0; i < list.Count; i++)
			{
				if (comparer.Equals(list[i], value))
				{
					return i;
				}
			}
			return -1;
		}

		public static bool Contains<T>(this IReadOnlyList<T> list, T value)
		{
			return list.IndexOf(value) >= 0;
		}

		public static int FindIndex<T>(this IReadOnlyList<T> list, Func<T, bool> predicate)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (predicate(list[i]))
				{
					return i;
				}
			}
			return -1;
		}
	}
}