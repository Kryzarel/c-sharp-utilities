using System;
using System.Collections.Generic;

namespace Kryz.Utils
{
	/// <summary>
	/// Hybrid sorting algorithm that uses <see cref="MergeSort"/> and switches to <see cref="BinarySort"/> on small data sets.
	/// <para>This sorting algorithm is stable.</para>
	/// </summary>
	public static class MergeBinarySort
	{
		public const int MergeThreshold = 64;

		// Array overloads allow us to use this class as a drop-in replacement for Array.Sort
		public static void Sort<T>(T[] data) => Sort(data.AsSpan(), Comparer<T>.Default);
		public static void Sort<T>(T[] data, int index, int length) => Sort(data.AsSpan(index, length), Comparer<T>.Default);
		public static void Sort<T, TComparer>(T[] data, TComparer comparer) where TComparer : IComparer<T> => Sort(data.AsSpan(), comparer);
		public static void Sort<T, TComparer>(T[] data, int index, int length, TComparer comparer) where TComparer : IComparer<T> => Sort(data.AsSpan(index, length), comparer);

		public static void Sort<T>(Span<T> data) => Sort(data, Comparer<T>.Default);

		public static void Sort<T, TComparer>(Span<T> data, TComparer comparer) where TComparer : IComparer<T>
		{
			int length = data.Length;

			if (length < MergeThreshold)
			{
				BinarySort.Sort(data, comparer);
			}
			else
			{
				int middle = length / 2;
				Sort(data[..middle], comparer);
				Sort(data[middle..], comparer);
				MergeSort.Merge(data, middle, comparer);
			}
		}

		public static void Sort<T>(IList<T> data) => Sort(data, 0, data.Count, Comparer<T>.Default);
		public static void Sort<T>(IList<T> data, int index, int length) => Sort(data, index, length, Comparer<T>.Default);
		public static void Sort<T, TComparer>(IList<T> data, TComparer comparer) where TComparer : IComparer<T> => Sort(data, 0, data.Count, comparer);

		public static void Sort<T, TComparer>(IList<T> data, int index, int length, TComparer comparer) where TComparer : IComparer<T> => SortRecursive(data, index, index + length - 1, comparer);

		public static void SortRecursive<T, TComparer>(IList<T> data, int left, int right, TComparer comparer) where TComparer : IComparer<T>
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