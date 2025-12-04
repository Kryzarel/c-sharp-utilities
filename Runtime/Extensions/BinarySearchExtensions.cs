using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Kryz.Utils
{
	public static class BinarySearchExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int BinarySearch<T>(this IReadOnlyList<T> list, T value)
		{
			return list.BinarySearch(value, 0, list.Count, Comparer<T>.Default);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int BinarySearch<T>(this IReadOnlyList<T> list, T value, int index, int count)
		{
			return list.BinarySearch(value, index, count, Comparer<T>.Default);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int BinarySearch<T, TComp>(this IReadOnlyList<T> list, T value, TComp comparer) where TComp : IComparer<T>
		{
			return list.BinarySearch(value, 0, list.Count, comparer);
		}

		public static int BinarySearch<T, TComp>(this IReadOnlyList<T> list, T value, int index, int count, TComp comparer) where TComp : IComparer<T>
		{
			return Utils.BinarySearch.Search(list, value, index, count, comparer);
		}
	}
}