using System;
using System.Collections.Generic;

namespace Kryz.Utils
{
	/// <summary>
	/// Binary Insertion Sort in C#. Generally faster than traditional Insertion Sort, especially for small data sets.
	/// <para>This sorting algorithm is stable. As long as we perform a rightmost binary search.</para>
	/// </summary>
	public static class BinarySort
	{
		// Array overloads allow us to use this class as a drop-in replacement for Array.Sort
		public static void Sort<T>(T[] data) => Sort(data.AsSpan(), Comparer<T>.Default);
		public static void Sort<T>(T[] data, int index, int length) => Sort(data.AsSpan(index, length), Comparer<T>.Default);
		public static void Sort<T, TComparer>(T[] data, TComparer comparer) where TComparer : IComparer<T> => Sort(data.AsSpan(), comparer);
		public static void Sort<T, TComparer>(T[] data, int index, int length, TComparer comparer) where TComparer : IComparer<T> => Sort(data.AsSpan(index, length), comparer);

		public static void Sort<T>(Span<T> data) => Sort(data, Comparer<T>.Default);

		public static void Sort<T, TComparer>(Span<T> data, TComparer comparer, int sorted = 1) where TComparer : IComparer<T>
		{
			for (int i = sorted; i < data.Length; i++)
			{
				T x = data[i];

				// Find location to insert using binary search
				int j = BinarySearch.Rightmost(data[..i], x, comparer);

				// Shift data one position to the right
				for (int k = i; k > j; k--)
				{
					data[k] = data[k - 1];
				}

				// Place element at its correct location
				data[j] = x;
			}
		}

		public static void Sort<T>(IList<T> data) => Sort(data, 0, data.Count, Comparer<T>.Default);
		public static void Sort<T>(IList<T> data, int index, int length) => Sort(data, index, length, Comparer<T>.Default);
		public static void Sort<T, TComparer>(IList<T> data, TComparer comparer) where TComparer : IComparer<T> => Sort(data, 0, data.Count, comparer);

		public static void Sort<T, TComparer>(IList<T> data, int index, int length, TComparer comparer, int sorted = 1) where TComparer : IComparer<T>
		{
			for (int i = index + sorted; i < index + length; i++)
			{
				T x = data[i];

				// Find location to insert using binary search
				int j = BinarySearch.Rightmost(data, index, i - index, x, comparer);

				// Shift data one position to the right
				for (int k = i; k > j; k--)
				{
					data[k] = data[k - 1];
				}

				// Place element at its correct location
				data[j] = x;
			}
		}
	}
}