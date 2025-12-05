using System.Collections.Generic;

namespace Kryz.Utils
{
	public static class BinarySearch
	{
		public static int Search<T>(IList<T> data, T value) => Search(data, 0, data.Count, value, Comparer<T>.Default);
		public static int Search<T>(IList<T> data, int index, int length, T value) => Search(data, index, length, value, Comparer<T>.Default);
		public static int Search<T, TComp>(IList<T> data, T value, TComp comparer) where TComp : IComparer<T> => Search(data, 0, data.Count, value, comparer);

		public static int Search<T, TComp>(IList<T> data, int index, int length, T value, TComp comparer) where TComp : IComparer<T>
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
		public static int Search<T, TComp>(IReadOnlyList<T> data, T value, TComp comparer) where TComp : IComparer<T> => Search(data, 0, data.Count, value, comparer);

		public static int Search<T, TComp>(IReadOnlyList<T> data, int index, int length, T value, TComp comparer) where TComp : IComparer<T>
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
	}
}