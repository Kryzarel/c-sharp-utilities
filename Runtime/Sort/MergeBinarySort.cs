using System.Collections.Generic;

namespace Kryz.Utils
{
	/// <summary>
	/// A sorting algorithm implementation that uses Merge Sort on large data sets (larger than '<see cref="MergeThreshold"/>') and switches to Binary Sort on small data sets.
	/// <para>This sorting algorithm is stable.</para>
	/// </summary>
	public static class MergeBinarySort
	{
		public const int MergeThreshold = 64;

		public static void Sort<T>(T[] data) => Sort(data, 0, data.Length, Comparer<T>.Default);
		public static void Sort<T>(T[] data, int index, int length) => Sort(data, index, length, Comparer<T>.Default);
		public static void Sort<T>(T[] data, IComparer<T> comparer) => Sort(data, 0, data.Length, comparer);

		public static void Sort<T>(T[] data, int index, int length, IComparer<T> comparer) => SortRecursive(data, index, index + length - 1, comparer);

		public static void SortRecursive<T>(T[] data, int left, int right, IComparer<T> comparer)
		{
			int count = right - left + 1;

			if (count < MergeThreshold)
			{
				BinarySort.Sort(data, left, count, comparer);
			}
			else
			{
				int middle = left + (right - left) / 2;
				SortRecursive(data, left, middle, comparer);
				SortRecursive(data, middle + 1, right, comparer);
				MergeSort.Merge(data, left, middle, right, comparer);
			}
		}

		public static void Sort<T>(IList<T> data) => Sort(data, 0, data.Count, Comparer<T>.Default);
		public static void Sort<T>(IList<T> data, int index, int length) => Sort(data, index, length, Comparer<T>.Default);
		public static void Sort<T>(IList<T> data, IComparer<T> comparer) => Sort(data, 0, data.Count, comparer);

		public static void Sort<T>(IList<T> data, int index, int length, IComparer<T> comparer) => SortRecursive(data, index, index + length - 1, comparer);

		public static void SortRecursive<T>(IList<T> data, int left, int right, IComparer<T> comparer)
		{
			int count = right - left + 1;

			if (count < MergeThreshold)
			{
				BinarySort.Sort(data, left, count, comparer);
			}
			else
			{
				int middle = left + (right - left) / 2;
				SortRecursive(data, left, middle, comparer);
				SortRecursive(data, middle + 1, right, comparer);
				MergeSort.Merge(data, left, middle, right, comparer);
			}
		}
	}
}