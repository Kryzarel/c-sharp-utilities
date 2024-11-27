using System;
using System.Collections.Generic;

namespace Kryz.Utils
{
	public static class BinarySearch
	{
		public static int BinarySearchLeftmost<T>(this IReadOnlyList<T> list, T value) => BinarySearchLeftmost(list, value, 0, list.Count);

		public static int BinarySearchLeftmost<T>(this IReadOnlyList<T> list, T value, int count) => BinarySearchLeftmost(list, value, 0, count);

		public static int BinarySearchLeftmost<T>(this IReadOnlyList<T> list, T value, int index, int count, IComparer<T>? comparer = null)
		{
			comparer ??= Comparer<T>.Default;
			int min = index;
			int max = count;

			while (min < max)
			{
				int mid = (min + max) / 2;

				if (comparer.Compare(list[mid], value) < 0)
					min = mid + 1;
				else
					max = mid;
			}
			return min;
		}

		public static int BinarySearchLeftmost<T>(this IReadOnlyList<T> list, Func<T, int> predicate) => BinarySearchLeftmost(list, 0, list.Count, predicate);

		public static int BinarySearchLeftmost<T>(this IReadOnlyList<T> list, int index, int count, Func<T, int> predicate)
		{
			int min = index;
			int max = count;

			while (min < max)
			{
				int mid = (min + max) / 2;

				if (predicate(list[mid]) < 0)
					min = mid + 1;
				else
					max = mid;
			}
			return min;
		}
	}
}