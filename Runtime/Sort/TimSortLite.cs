using System;
using System.Collections.Generic;

namespace Kryz.Utils
{
	/// <summary>
	/// Using some concepts from TimSort, we get a (slightly) more optimized version of <see cref="MergeBinarySort"/> while avoiding the complexity of implementing TimSort's full 600+ lines.
	/// <para>This sorting algorithm is stable.</para>
	/// </summary>
	public static class TimSortLite
	{
		// Array overloads allow us to use this class as a drop-in replacement for Array.Sort
		public static void Sort<T>(T[] data) => Sort(data.AsSpan(), Comparer<T>.Default);
		public static void Sort<T>(T[] data, IComparer<T> comparer) => Sort(data.AsSpan(), comparer);
		public static void Sort<T>(T[] data, int index, int length) => Sort(data.AsSpan(index, length), Comparer<T>.Default);
		public static void Sort<T>(T[] data, int index, int length, IComparer<T> comparer) => Sort(data.AsSpan(index, length), comparer);

		public static void Sort<T>(Span<T> data) => Sort(data, Comparer<T>.Default);

		public static void Sort<T>(Span<T> data, IComparer<T> comparer)
		{
			int length = data.Length;
			int minRun = ComputeMinRun(data.Length);

			// Sort small runs using Binary Insertion Sort
			for (int left = 0; left < length; left += minRun)
			{
				int len = Math.Min(minRun, length - left);
				BinarySort.Sort(data[left..(left + len)], comparer);
			}

			// Merge runs in powers of two: minRun -> 2*minRun -> 4*minRun -> ...
			for (int size = minRun; size < length; size *= 2)
			{
				for (int left = 0; left + size < length; left += 2 * size)
				{
					int right = Math.Min(left + 2 * size, length);
					MergeSort.Merge(data[left..right], size, comparer);
				}
			}
		}

		public static void Sort<T>(IList<T> data) => Sort(data, 0, data.Count, Comparer<T>.Default);
		public static void Sort<T>(IList<T> data, IComparer<T> comparer) => Sort(data, 0, data.Count, comparer);
		public static void Sort<T>(IList<T> data, int index, int length) => Sort(data, index, length, Comparer<T>.Default);

		public static void Sort<T>(IList<T> data, int index, int length, IComparer<T> comparer)
		{
			int end = index + length;
			int minRun = ComputeMinRun(length);

			// Sort small runs using Binary Insertion Sort
			for (int left = index; left < end; left += minRun)
			{
				int len = Math.Min(minRun, end - left);
				BinarySort.Sort(data, left, len, comparer);
			}

			// Merge runs in powers of two: minRun -> 2*minRun -> 4*minRun -> ...
			for (int size = minRun; size < length; size *= 2)
			{
				for (int left = index; left + size < end; left += 2 * size)
				{
					int mid = left + size - 1;
					int right = Math.Min(left + 2 * size - 1, end - 1);
					MergeSort.Merge(data, left, mid, right, comparer);
				}
			}
		}

		public static int ComputeMinRun(int n)
		{
			// TimSort's minrun calculation. Returns a value between 32 and 64.
			int r = 0;
			while (n >= 64)
			{
				r |= n & 1;
				n >>= 1;
			}
			return n + r;
		}
	}
}