using System;
using System.Collections.Generic;

namespace Kryz.Utils
{
	public static class ListExtensions
	{
		public static void EnsureCapacity<T>(this List<T> list, int capacity)
		{
			if (list.Capacity.TryGetNewCapacity(capacity, out int newCapacity))
			{
				list.Capacity = newCapacity;
			}
		}

		public static void AddRangeNonAlloc<T>(this List<T> list, ReadOnlySpan<T> span)
		{
			list.EnsureCapacity(list.Count + span.Length);
			list.AddRangeFromSpan(span);
		}

		public static void AddRangeNonAlloc<T, TAdd>(this List<T> list, TAdd toAdd) where TAdd : IReadOnlyList<T>
		{
			list.EnsureCapacity(list.Count + toAdd.Count);
			list.AddRangeFromIReadOnlyList<T, List<T>, TAdd>(toAdd);
		}
	}
}