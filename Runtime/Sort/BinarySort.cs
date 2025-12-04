using System;
using System.Collections.Generic;

namespace Kryz.Utils
{
	public static class BinarySort
	{
		public static void Sort<T>(T[] array) => Sort(array, 0, array.Length, Comparer<T>.Default);
		public static void Sort<T>(T[] array, IComparer<T> comparer) => Sort(array, 0, array.Length, comparer);
		public static void Sort<T>(T[] array, int start, int count) => Sort(array, start, count, Comparer<T>.Default);

		public static void Sort<T>(T[] array, int start, int count, IComparer<T> comparer, int sorted = 1)
		{
			for (int i = start + sorted; i < start + count; i++)
			{
				T x = array[i];

				// Find location to insert using binary search
				int j = Array.BinarySearch(array, start, i - start, x, comparer);
				if (j < 0) j = ~j;

				// Shifting array to one location right
				Array.Copy(array, j, array, j + 1, i - j);

				// Placing element at its correct location
				array[j] = x;
			}
		}
	}
}