using System;
using System.Collections.Generic;

namespace Kryz.Utils
{
	public static class ListExtensions
	{
		public static void EnsureCapacity<T>(this List<T> list, int capacity)
		{
			int current = list.Capacity;

			if (capacity > current)
			{
				int newCapacity = current * 2;
				// Note that this check works even when newCapacity overflowed thanks to the (uint) cast
				if ((uint)newCapacity > int.MaxValue) newCapacity = int.MaxValue;
				// If the computed capacity is still less than specified, set to the original argument.
				if (newCapacity < capacity) newCapacity = capacity;

				list.Capacity = newCapacity;
			}
		}

		public static void AddRangeNonAlloc<T>(this List<T> list, ReadOnlySpan<T> span)
		{
			list.EnsureCapacity(list.Count + span.Length);
			((IList<T>)list).AddRangeNonAlloc(span);
		}

		public static void AddRangeNonAlloc<T, TAdd>(this List<T> list, IList<T> toAdd)
		{
			list.EnsureCapacity(list.Count + toAdd.Count);
			list.AddRangeNonAlloc(toAdd);
		}

		public static void AddRangeNonAlloc<T, TAdd>(this List<T> list, IReadOnlyList<T> toAdd)
		{
			list.EnsureCapacity(list.Count + toAdd.Count);
			list.AddRangeNonAlloc(toAdd);
		}
	}
}