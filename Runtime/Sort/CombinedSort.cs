using System.Collections.Generic;

namespace Kryz.Utils
{
	public static class CombinedSort
	{
		public const int Threshold = 64;

		public static void Sort<T>(T[] array) => Sort(array, 0, array.Length, Comparer<T>.Default);
		public static void Sort<T>(T[] array, IComparer<T> comparer) => Sort(array, 0, array.Length, comparer);
		public static void Sort<T>(T[] array, int start, int count) => Sort(array, start, count, Comparer<T>.Default);

		public static void Sort<T>(T[] array, int start, int count, IComparer<T> comparer) => SortRecursive(array, start, start + count - 1, comparer);

		public static void SortRecursive<T>(T[] array, int left, int right, IComparer<T> comparer)
		{
			int count = right - left + 1;

			if (count < Threshold)
			{
				BinarySort.Sort(array, left, count, comparer);
			}
			else
			{
				int middle = left + (right - left) / 2;
				SortRecursive(array, left, middle, comparer);
				SortRecursive(array, middle + 1, right, comparer);
				MergeSort.Merge(array, left, middle, right, comparer);
			}
		}
	}
}