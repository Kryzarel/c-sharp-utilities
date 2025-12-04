using System;
using System.Buffers;
using System.Collections.Generic;

namespace Kryz.Utils
{
	/// <summary>
	/// Implementation of Merge Sort in C# using ArrayPool for the intermediate arrays to reduce heap allocations.
	/// </summary>
	public static class MergeSort
	{
		public static void Sort<T>(T[] array) => Sort(array, 0, array.Length, Comparer<T>.Default);
		public static void Sort<T>(T[] array, IComparer<T> comparer) => Sort(array, 0, array.Length, comparer);
		public static void Sort<T>(T[] array, int start, int count) => Sort(array, start, count, Comparer<T>.Default);

		public static void Sort<T>(T[] array, int start, int count, IComparer<T> comparer)
		{
			int left = start;
			int right = count - 1;

			if (left < right)
			{
				int middle = left + (right - left) / 2;
				Sort(array, left, middle + 1, comparer);
				Sort(array, middle + 1, right + 1, comparer);
				Merge(array, left, middle, right, comparer);
			}
		}

		private static void Merge<T>(T[] array, int left, int middle, int right, IComparer<T> comparer)
		{
			int leftArrayLength = middle - left + 1;
			int rightArrayLength = right - middle;
			T[] leftTempArray = ArrayPool<T>.Shared.Rent(leftArrayLength);
			T[] rightTempArray = ArrayPool<T>.Shared.Rent(rightArrayLength);

			Array.Copy(array, left, leftTempArray, 0, leftArrayLength);
			Array.Copy(array, middle + 1, rightTempArray, 0, rightArrayLength);

			int i = 0, j = 0, k = left;

			while (i < leftArrayLength && j < rightArrayLength)
			{
				if (comparer.Compare(leftTempArray[i], rightTempArray[j]) <= 0)
				{
					array[k++] = leftTempArray[i++];
				}
				else
				{
					array[k++] = rightTempArray[j++];
				}
			}

			while (i < leftArrayLength)
			{
				array[k++] = leftTempArray[i++];
			}

			while (j < rightArrayLength)
			{
				array[k++] = rightTempArray[j++];
			}

			Array.Clear(leftTempArray, 0, leftArrayLength);
			Array.Clear(rightTempArray, 0, rightArrayLength);

			ArrayPool<T>.Shared.Return(leftTempArray);
			ArrayPool<T>.Shared.Return(rightTempArray);
		}
	}
}