using System.Collections.Generic;

namespace Kryz.Utils.Tests
{
	public class InsertionSortTests : SortTests
	{
		protected class Algorithm : SortAlgorithm
		{
			public override void Sort<T>(T[] data) => InsertionSort.Sort(data);
			public override void Sort<T>(T[] data, IComparer<T> comp) => InsertionSort.Sort(data, comp);
			public override void Sort<T>(T[] data, int index, int length) => InsertionSort.Sort(data, index, length);
			public override void Sort<T>(T[] data, int index, int length, IComparer<T> comp) => InsertionSort.Sort(data, index, length, comp);

			public override void Sort<T>(IList<T> data) => InsertionSort.Sort(data);
			public override void Sort<T>(IList<T> data, IComparer<T> comp) => InsertionSort.Sort(data, comp);
			public override void Sort<T>(IList<T> data, int index, int length) => InsertionSort.Sort(data, index, length);
			public override void Sort<T>(IList<T> data, int index, int length, IComparer<T> comp) => InsertionSort.Sort(data, index, length, comp);
		}

		protected override SortAlgorithm Sorter { get; } = new Algorithm();
	}
}