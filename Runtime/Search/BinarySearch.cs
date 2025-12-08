using System;
using System.Collections.Generic;

namespace Kryz.Utils
{
	public static class BinarySearch
	{
		public static int Search<T>(IList<T> data, T value) => Search(data, 0, data.Count, value, Comparer<T>.Default);
		public static int Search<T>(IList<T> data, int index, int length, T value) => Search(data, index, length, value, Comparer<T>.Default);
		public static int Search<T, TComparer>(IList<T> data, T value, TComparer comparer) where TComparer : IComparer<T> => Search(data, 0, data.Count, value, comparer);

		public static int Search<T, TComparer>(IList<T> data, int index, int length, T value, TComparer comparer) where TComparer : IComparer<T>
		{
			int min = index;
			int max = index + length - 1;

			while (min <= max)
			{
				int mid = min + ((max - min) >> 1);
				int result = comparer.Compare(data[mid], value);

				if (result == 0)
					return mid;
				else if (result < 0)
					min = mid + 1;
				else
					max = mid - 1;
			}
			return ~min;
		}

		public static int Search<T>(IReadOnlyList<T> data, T value) => Search(data, 0, data.Count, value, Comparer<T>.Default);
		public static int Search<T>(IReadOnlyList<T> data, int index, int length, T value) => Search(data, index, length, value, Comparer<T>.Default);
		public static int Search<T, TComparer>(IReadOnlyList<T> data, T value, TComparer comparer) where TComparer : IComparer<T> => Search(data, 0, data.Count, value, comparer);

		public static int Search<T, TComparer>(IReadOnlyList<T> data, int index, int length, T value, TComparer comparer) where TComparer : IComparer<T>
		{
			int min = index;
			int max = index + length - 1;

			while (min <= max)
			{
				int mid = min + ((max - min) >> 1);
				int result = comparer.Compare(data[mid], value);

				if (result == 0)
					return mid;
				else if (result < 0)
					min = mid + 1;
				else
					max = mid - 1;
			}
			return ~min;
		}

		// -------------------------------------
		// Required for BinarySort to be stable
		// -------------------------------------

		// Array overloads allow us to use this class as a drop-in replacement for Array.BinarySearch
		public static int Rightmost<T>(T[] data, T value) => Rightmost(data.AsSpan(), value, Comparer<T>.Default);
		public static int Rightmost<T>(T[] data, int index, int length, T value) => Rightmost(data.AsSpan(index, length), value, Comparer<T>.Default);
		public static int Rightmost<T, TComparer>(T[] data, T value, TComparer comparer) where TComparer : IComparer<T> => Rightmost(data.AsSpan(), value, comparer);
		public static int Rightmost<T, TComparer>(T[] data, int index, int length, T value, TComparer comparer) where TComparer : IComparer<T> => Rightmost(data.AsSpan(index, length), value, comparer);

		public static int Rightmost<T>(Span<T> data, T value) => Rightmost(data, value, Comparer<T>.Default);

		public static int Rightmost<T, TComparer>(Span<T> data, T value, TComparer comparer) where TComparer : IComparer<T>
		{
			int min = 0;
			int max = data.Length - 1;

			while (min <= max)
			{
				int mid = min + ((max - min) >> 1);

				// Doesn't return early for '== 0' case to preserve stability
				if (comparer.Compare(data[mid], value) <= 0)
					min = mid + 1;
				else
					max = mid - 1;
			}
			return min;
		}

		public static int Rightmost<T>(IList<T> data, T value) => Rightmost(data, 0, data.Count, value, Comparer<T>.Default);
		public static int Rightmost<T>(IList<T> data, int index, int length, T value) => Rightmost(data, index, length, value, Comparer<T>.Default);
		public static int Rightmost<T, TComparer>(IList<T> data, T value, TComparer comparer) where TComparer : IComparer<T> => Rightmost(data, 0, data.Count, value, comparer);

		public static int Rightmost<T, TComparer>(IList<T> data, int index, int length, T value, TComparer comparer) where TComparer : IComparer<T>
		{
			int min = index;
			int max = index + length - 1;

			while (min <= max)
			{
				int mid = min + ((max - min) >> 1);

				// Doesn't return early for '== 0' case to preserve stability
				if (comparer.Compare(data[mid], value) <= 0)
					min = mid + 1;
				else
					max = mid - 1;
			}
			return min;
		}

		public static int Rightmost<T>(IReadOnlyList<T> data, T value) => Rightmost(data, 0, data.Count, value, Comparer<T>.Default);
		public static int Rightmost<T>(IReadOnlyList<T> data, int index, int length, T value) => Rightmost(data, index, length, value, Comparer<T>.Default);
		public static int Rightmost<T, TComparer>(IReadOnlyList<T> data, T value, TComparer comparer) where TComparer : IComparer<T> => Rightmost(data, 0, data.Count, value, comparer);

		public static int Rightmost<T, TComparer>(IReadOnlyList<T> data, int index, int length, T value, TComparer comparer) where TComparer : IComparer<T>
		{
			int min = index;
			int max = index + length - 1;

			while (min <= max)
			{
				int mid = min + ((max - min) >> 1);

				// Doesn't return early for '== 0' case to preserve stability
				if (comparer.Compare(data[mid], value) <= 0)
					min = mid + 1;
				else
					max = mid - 1;
			}
			return min;
		}
	}
}