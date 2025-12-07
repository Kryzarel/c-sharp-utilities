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
		// Array overloads allow us to use this class as a drop-in replacement for Array.Sort
		public static void Sort<T>(T[] data) => Sort(data.AsSpan(), Comparer<T>.Default);
		public static void Sort<T>(T[] data, int index, int length) => Sort(data.AsSpan(index, length), Comparer<T>.Default);
		public static void Sort<T, TComparer>(T[] data, TComparer comparer) where TComparer : IComparer<T> => Sort(data.AsSpan(), comparer);
		public static void Sort<T, TComparer>(T[] data, int index, int length, TComparer comparer) where TComparer : IComparer<T> => Sort(data.AsSpan(index, length), comparer);

		public static void Sort<T>(Span<T> data) => Sort(data, Comparer<T>.Default);

		public static void Sort<T, TComparer>(Span<T> data, TComparer comparer) where TComparer : IComparer<T>
		{
			int length = data.Length;

			if (length > 1)
			{
				int middle = length >> 1;
				Sort(data[..middle], comparer);
				Sort(data[middle..], comparer);
				Merge(data, middle, comparer);
			}
		}

		public static void Merge<T, TComparer>(Span<T> data, int middle, TComparer comparer) where TComparer : IComparer<T>
		{
			int right = data.Length - 1;
			int leftLength = middle;
			T[] leftTemp = ArrayPool<T>.Shared.Rent(leftLength);

			data[..leftLength].CopyTo(leftTemp);

			int i = 0; // index in leftTemp
			int j = middle; // index in right half of data
			int k = 0; // index to write in data

			// Merge from leftTemp and right half in data
			while (i < leftLength && j <= right)
			{
				data[k++] = comparer.Compare(leftTemp[i], data[j]) <= 0 ? leftTemp[i++] : data[j++];
			}

			// Copy any remaining elements from leftTemp (right half is already in place)
			while (i < leftLength)
			{
				data[k++] = leftTemp[i++];
			}

			Array.Clear(leftTemp, 0, leftLength);
			ArrayPool<T>.Shared.Return(leftTemp);
		}

		public static void Sort<T>(IList<T> data) => Sort(data, 0, data.Count, Comparer<T>.Default);
		public static void Sort<T>(IList<T> data, IComparer<T> comparer) => Sort(data, 0, data.Count, comparer);
		public static void Sort<T>(IList<T> data, int index, int length) => Sort(data, index, length, Comparer<T>.Default);

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
			int leftLength = middle - left + 1;
			T[] leftTemp = ArrayPool<T>.Shared.Rent(leftLength);

			for (int l = 0; l < leftLength; l++)
			{
				leftTemp[l] = data[left + l];
			}

			int i = 0; // index in leftTemp
			int j = middle + 1; // index in right half of data
			int k = left; // index to write in data

			// Merge from leftTemp and right half in data
			while (i < leftLength && j <= right)
			{
				data[k++] = comparer.Compare(leftTemp[i], data[j]) <= 0 ? leftTemp[i++] : data[j++];
			}

			// Copy any remaining elements from leftTemp (right half is already in place)
			while (i < leftLength)
			{
				data[k++] = leftTemp[i++];
			}

			Array.Clear(leftTemp, 0, leftLength);
			ArrayPool<T>.Shared.Return(leftTemp);
		}
	}
}