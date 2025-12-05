using System;
using System.Buffers;
using System.Collections.Generic;

namespace Kryz.Utils
{
	/// <summary>
	/// Implementation of Merge Sort in C# using <see cref="ArrayPool{T}"/> for the intermediate arrays to reduce heap allocations.
	/// <para>This sorting algorithm is stable.</para>
	/// </summary>
	public static class MergeSort
	{
		public static void Sort<T>(T[] data) => Sort(data, 0, data.Length, Comparer<T>.Default);
		public static void Sort<T>(T[] data, int index, int length) => Sort(data, index, length, Comparer<T>.Default);
		public static void Sort<T>(T[] data, IComparer<T> comparer) => Sort(data, 0, data.Length, comparer);

		public static void Sort<T>(T[] data, int index, int length, IComparer<T> comparer) => SortRecursive(data, index, index + length - 1, comparer);

		public static void SortRecursive<T>(T[] data, int left, int right, IComparer<T> comparer)
		{
			if (left < right)
			{
				int middle = left + (right - left) / 2;
				SortRecursive(data, left, middle, comparer);
				SortRecursive(data, middle + 1, right, comparer);
				Merge(data, left, middle, right, comparer);
			}
		}

		public static void Merge<T>(T[] data, int left, int middle, int right, IComparer<T> comparer)
		{
			int leftArrayLength = middle - left + 1;
			int rightArrayLength = right - middle;
			T[] leftTempArray = ArrayPool<T>.Shared.Rent(leftArrayLength);
			T[] rightTempArray = ArrayPool<T>.Shared.Rent(rightArrayLength);

			Array.Copy(data, left, leftTempArray, 0, leftArrayLength);
			Array.Copy(data, middle + 1, rightTempArray, 0, rightArrayLength);

			int i = 0, j = 0, k = left;

			while (i < leftArrayLength && j < rightArrayLength)
			{
				if (comparer.Compare(leftTempArray[i], rightTempArray[j]) <= 0)
				{
					data[k++] = leftTempArray[i++];
				}
				else
				{
					data[k++] = rightTempArray[j++];
				}
			}

			while (i < leftArrayLength)
			{
				data[k++] = leftTempArray[i++];
			}

			while (j < rightArrayLength)
			{
				data[k++] = rightTempArray[j++];
			}

			Array.Clear(leftTempArray, 0, leftArrayLength);
			Array.Clear(rightTempArray, 0, rightArrayLength);

			ArrayPool<T>.Shared.Return(leftTempArray);
			ArrayPool<T>.Shared.Return(rightTempArray);
		}

		public static void Sort<T>(IList<T> data) => Sort(data, 0, data.Count, Comparer<T>.Default);
		public static void Sort<T>(IList<T> data, int index, int length) => Sort(data, index, length, Comparer<T>.Default);
		public static void Sort<T>(IList<T> data, IComparer<T> comparer) => Sort(data, 0, data.Count, comparer);

		public static void Sort<T>(IList<T> data, int index, int length, IComparer<T> comparer) => SortRecursive(data, index, index + length - 1, comparer);

		public static void SortRecursive<T>(IList<T> data, int left, int right, IComparer<T> comparer)
		{
			if (left < right)
			{
				int middle = left + (right - left) / 2;
				SortRecursive(data, left, middle, comparer);
				SortRecursive(data, middle + 1, right, comparer);
				Merge(data, left, middle, right, comparer);
			}
		}

		public static void Merge<T>(IList<T> data, int left, int middle, int right, IComparer<T> comparer)
		{
			int leftArrayLength = middle - left + 1;
			int rightArrayLength = right - middle;
			T[] leftTempArray = ArrayPool<T>.Shared.Rent(leftArrayLength);
			T[] rightTempArray = ArrayPool<T>.Shared.Rent(rightArrayLength);

			int i, j;
			for (i = 0; i < leftArrayLength; ++i)
				leftTempArray[i] = data[left + i];
			for (j = 0; j < rightArrayLength; ++j)
				rightTempArray[j] = data[middle + 1 + j];

			i = j = 0;
			int k = left;

			while (i < leftArrayLength && j < rightArrayLength)
			{
				if (comparer.Compare(leftTempArray[i], rightTempArray[j]) <= 0)
				{
					data[k++] = leftTempArray[i++];
				}
				else
				{
					data[k++] = rightTempArray[j++];
				}
			}

			while (i < leftArrayLength)
			{
				data[k++] = leftTempArray[i++];
			}

			while (j < rightArrayLength)
			{
				data[k++] = rightTempArray[j++];
			}

			Array.Clear(leftTempArray, 0, leftArrayLength);
			Array.Clear(rightTempArray, 0, rightArrayLength);

			ArrayPool<T>.Shared.Return(leftTempArray);
			ArrayPool<T>.Shared.Return(rightTempArray);
		}
	}
}