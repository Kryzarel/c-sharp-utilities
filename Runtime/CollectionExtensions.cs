using System;
using System.Collections.Generic;

namespace Kryz.SharpUtils
{
	public static class CollectionExtensions
	{
		public static void EnsureCapacity<T>(this List<T> list, int capacity)
		{
			int currentCapacity = list.Capacity;
			if (capacity > currentCapacity)
			{
				int newCapacity = (int)Math.Min(int.MaxValue, (uint)currentCapacity * 2);
				list.Capacity = Math.Max(capacity, newCapacity);
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

		public static bool Contains<T>(this IReadOnlyList<T?> list, T value) where T : class
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i] == value)
				{
					return true;
				}
			}
			return false;
		}
	}
}