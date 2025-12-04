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
				int min = start;
				int max = i;
				T pivot = array[max];

				// binary search for insertion point
				while (min < max)
				{
					int mid = min + ((max - min) >> 1);

					if (comparer.Compare(pivot, array[mid]) < 0)
					{
						max = mid;
					}
					else
					{
						min = mid + 1;
					}
				}

				// shift right to make room
				for (int p = i; p > min; p--)
				{
					array[p] = array[p - 1];
				}

				array[min] = pivot;
			}
		}
	}
}