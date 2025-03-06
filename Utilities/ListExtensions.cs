using System;
using System.Collections.Generic;

namespace Kryz.Utils
{
	public static class ListExtensions
	{
		public static void EnsureCapacity<T>(this List<T> list, int capacity)
		{
			if (list.Capacity.TryEnsureCapacity(capacity, out int newCapacity))
			{
				list.Capacity = newCapacity;
			}
		}

		public static void AddRange<T>(this List<T> list, Span<T> span)
		{
			list.EnsureCapacity(list.Count + span.Length);
			list.AddRange<T, List<T>>(span);
		}

		public static void AddRangeNonAlloc<T, TAdd>(this List<T> list, TAdd toAdd) where TAdd : IReadOnlyList<T>
		{
			list.EnsureCapacity(list.Count + toAdd.Count);
			list.AddRangeNonAlloc<T, List<T>, TAdd>(toAdd);
		}

		public static void AddRangeWhere<T, TAdd>(this List<T> list, TAdd toAdd, Func<T, bool> predicate) where TAdd : IReadOnlyList<T>
		{
			list.EnsureCapacity(list.Count + toAdd.Count);
			list.AddRangeWhere<T, List<T>, TAdd>(toAdd, predicate);
		}
	}
}