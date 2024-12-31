using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Kryz.Utils
{
	public static class BinarySearch
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int BinarySearchLeftmost<T, TList>(this TList list, T value) where TList : IReadOnlyList<T>
		{
			return list.BinarySearchLeftmost(value, 0, list.Count, Comparer<T>.Default);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int BinarySearchLeftmost<T, TList>(this TList list, T value, int index, int count) where TList : IReadOnlyList<T>
		{
			return list.BinarySearchLeftmost(value, index, count, Comparer<T>.Default);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int BinarySearchLeftmost<T, TList, TComp>(this TList list, T value, TComp comparer) where TList : IReadOnlyList<T> where TComp : IComparer<T>
		{
			return list.BinarySearchLeftmost(value, 0, list.Count, comparer);
		}

		public static int BinarySearchLeftmost<T, TList, TComp>(this TList list, T value, int index, int count, TComp comparer) where TList : IReadOnlyList<T> where TComp : IComparer<T>
		{
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
	}
}