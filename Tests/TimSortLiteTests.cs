using System.Collections.Generic;

namespace Kryz.Utils.Tests
{
	public class TimSortLiteTests : SortTests
	{
		protected class Algorithm : SortAlgorithm
		{
			public override void Sort<T>(T[] data) => TimSortLite.Sort(data);
			public override void Sort<T>(T[] data, IComparer<T> comp) => TimSortLite.Sort(data, comp);
			public override void Sort<T>(T[] data, int index, int length) => TimSortLite.Sort(data, index, length);
			public override void Sort<T>(T[] data, int index, int length, IComparer<T> comp) => TimSortLite.Sort(data, index, length, comp);

			public override void Sort<T>(IList<T> data) => TimSortLite.Sort(data);
			public override void Sort<T>(IList<T> data, IComparer<T> comp) => TimSortLite.Sort(data, comp);
			public override void Sort<T>(IList<T> data, int index, int length) => TimSortLite.Sort(data, index, length);
			public override void Sort<T>(IList<T> data, int index, int length, IComparer<T> comp) => TimSortLite.Sort(data, index, length, comp);
		}

		protected override SortAlgorithm Sorter { get; } = new Algorithm();
	}
}