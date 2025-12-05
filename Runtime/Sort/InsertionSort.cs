using System.Collections.Generic;

namespace Kryz.Utils
{
	/// <summary>
	/// Implementation of Insertion Sort in C#. Fast for small data sets, although <see cref="BinarySort"/> is usually faster.
	/// <para>This sorting algorithm is stable.</para>
	/// </summary>
	public static class InsertionSort
	{
		public static void Sort<T>(T[] data) => Sort(data, 0, data.Length, Comparer<T>.Default);
		public static void Sort<T>(T[] data, int index, int length) => Sort(data, index, length, Comparer<T>.Default);
		public static void Sort<T>(T[] data, IComparer<T> comparer) => Sort(data, 0, data.Length, comparer);

		public static void Sort<T>(T[] data, int index, int length, IComparer<T> comparer)
		{
			for (int i = index + 1; i < index + length; i++)
			{
				T x = data[i];

				int j = i;
				for (; j > 0 && comparer.Compare(data[j - 1], x) > 0; j--)
				{
					data[j] = data[j - 1];
				}

				data[j] = x;
			}
		}

		public static void Sort<T>(IList<T> data) => Sort(data, 0, data.Count, Comparer<T>.Default);
		public static void Sort<T>(IList<T> data, int index, int length) => Sort(data, index, length, Comparer<T>.Default);
		public static void Sort<T>(IList<T> data, IComparer<T> comparer) => Sort(data, 0, data.Count, comparer);

		public static void Sort<T>(IList<T> data, int index, int length, IComparer<T> comparer)
		{
			for (int i = index + 1; i < index + length; i++)
			{
				T x = data[i];

				int j = i;
				for (; j > 0 && comparer.Compare(data[j - 1], x) > 0; j--)
				{
					data[j] = data[j - 1];
				}

				data[j] = x;
			}
		}
	}
}