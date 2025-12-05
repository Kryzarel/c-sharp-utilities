using System;
using System.Collections.Generic;

namespace Kryz.Utils
{
	/// <summary>
	/// Using some of the concepts of TimSort, we achieve a more optimized version of <see cref="MergeBinarySort"/> while still not implementing the full 600+ lines of TimSort in its entirety.
	/// <para>This sorting algorithm is stable.</para>
	/// </summary>
	public static class TimSortLite
	{
		public const int MinRun = 64;

		public static void Sort<T>(T[] data) => Sort(data, 0, data.Length, Comparer<T>.Default);
		public static void Sort<T>(T[] data, int index, int length) => Sort(data, index, length, Comparer<T>.Default);
		public static void Sort<T>(T[] data, IComparer<T> comparer) => Sort(data, 0, data.Length, comparer);

		public static void Sort<T>(T[] data, int index, int length, IComparer<T> comparer)
		{
			int end = index + length;

			// Sort small runs using Binary Insertion Sort
			for (int left = index; left < end; left += MinRun)
			{
				int len = Math.Min(MinRun, end - left);
				BinarySort.Sort(data, left, len, comparer);
			}

			// Merge runs in powers of two: MinRun -> 2*MinRun -> 4*MinRun -> ...
			for (int size = MinRun; size < length; size *= 2)
			{
				for (int left = index; left + size < end; left += 2 * size)
				{
					int mid = left + size - 1;
					int right = Math.Min(left + 2 * size - 1, end - 1);
					MergeSort.Merge(data, left, mid, right, comparer);
				}
			}
		}
	}
}