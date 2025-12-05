using System;
using System.Collections.Generic;

namespace Kryz.Utils
{
	/// <summary>
	/// Binary Insertion Sort in C#. Generally faster than traditional Insertion Sort, especially for small data sets.
	/// <para>This sorting algorithm is stable.</para>
	/// </summary>
	public static class BinarySort
	{
		public static void Sort<T>(T[] data) => Sort(data, 0, data.Length, Comparer<T>.Default);
		public static void Sort<T>(T[] data, int index, int length) => Sort(data, index, length, Comparer<T>.Default);
		public static void Sort<T>(T[] data, IComparer<T> comparer) => Sort(data, 0, data.Length, comparer);

		public static void Sort<T>(T[] data, int index, int length, IComparer<T> comparer, int sorted = 1)
		{
			for (int i = index + sorted; i < index + length; i++)
			{
				T x = data[i];

				// Find location to insert using binary search
				int j = Array.BinarySearch(data, index, i - index, x, comparer);
				if (j < 0) j = ~j;

				// Shift data one spot to the right
				Array.Copy(data, j, data, j + 1, i - j);

				// Place element at its correct location
				data[j] = x;
			}
		}

		public static void Sort<T>(IList<T> data) => Sort(data, 0, data.Count, Comparer<T>.Default);
		public static void Sort<T>(IList<T> data, int index, int length) => Sort(data, index, length, Comparer<T>.Default);
		public static void Sort<T>(IList<T> data, IComparer<T> comparer) => Sort(data, 0, data.Count, comparer);

		public static void Sort<T>(IList<T> data, int index, int length, IComparer<T> comparer, int sorted = 1)
		{
			for (int i = index + sorted; i < index + length; i++)
			{
				T x = data[i];

				// Find location to insert using binary search
				int j = BinarySearch.Search(data, index, i - index, x, comparer);
				if (j < 0) j = ~j;

				// Shift data one spot to the right
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