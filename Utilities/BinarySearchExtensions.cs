using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Kryz.Utils
{
	public static class BinarySearchExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int BinarySearch<T, TList>(this TList list, T value) where TList : IReadOnlyList<T>
		{
			return list.BinarySearch(value, 0, list.Count, Comparer<T>.Default);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int BinarySearch<T, TList>(this TList list, T value, int index, int count) where TList : IReadOnlyList<T>
		{
			return list.BinarySearch(value, index, count, Comparer<T>.Default);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int BinarySearch<T, TList, TComp>(this TList list, T value, TComp comparer) where TList : IReadOnlyList<T> where TComp : IComparer<T>
		{
			return list.BinarySearch(value, 0, list.Count, comparer);
		}

		public static int BinarySearch<T, TList, TComp>(this TList list, T value, int index, int count, TComp comparer) where TList : IReadOnlyList<T> where TComp : IComparer<T>
		{
			int min = index;
			int max = index + count - 1;

			while (min <= max)
			{
				int mid = min + ((max - min) >> 1);

				int result = comparer.Compare(list[mid], value);
				if (result == 0) return mid;

				if (result < 0)
					min = mid + 1;
				else
					max = mid - 1;
			}
			return ~min;
		}
	}
}