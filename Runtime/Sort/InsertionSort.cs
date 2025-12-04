using System.Collections.Generic;

namespace Kryz.Utils
{
	public static class InsertionSort
	{
		public static void Sort<T>(T[] array) => Sort(array, 0, array.Length, Comparer<T>.Default);
		public static void Sort<T>(T[] array, IComparer<T> comparer) => Sort(array, 0, array.Length, comparer);
		public static void Sort<T>(T[] array, int start, int count) => Sort(array, start, count, Comparer<T>.Default);

		public static void Sort<T>(T[] array, int start, int count, IComparer<T> comparer)
		{
			for (int i = start + 1; i < start + count; i++)
			{
				T x = array[i];

				int j = i;
				for (; j > 0 && comparer.Compare(array[j - 1], x) > 0; j--)
				{
					array[j] = array[j - 1];
				}

				array[j] = x;
			}
		}
	}
}