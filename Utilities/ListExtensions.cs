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
			((IList<T>)list).AddRangeNonAlloc(span);
		}

		public static void AddRangeNonAlloc<T, TAdd>(this List<T> list, TAdd toAdd) where TAdd : IReadOnlyList<T>
		{
			list.EnsureCapacity(list.Count + toAdd.Count);
			((IList<T>)list).AddRangeNonAlloc(toAdd);
		}
	}
}