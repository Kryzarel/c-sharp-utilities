using System.Collections.Generic;

namespace Kryz.Utils
{
	public static class BinarySearch
	{
		public static int Search<T, TComp>(IReadOnlyList<T> list, T value, int index, int count, TComp comparer) where TComp : IComparer<T>
		{
			int min = index;
			int max = index + count - 1;

			while (min <= max)
			{
				int mid = min + ((max - min) >> 1);

				int result = comparer.Compare(list[mid], value);
				if (result == 0) return mid;

				if (result < 0)
					min = mid + 1;
				else
					max = mid - 1;
			}
			return ~min;
		}
	}
}