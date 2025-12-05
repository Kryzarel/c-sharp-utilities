using System.Collections.Generic;

namespace Kryz.Utils.Tests
{
	public class MergeSortTests : SortTests
	{
		protected class Algorithm : SortAlgorithm
		{
			public override void Sort<T>(T[] data) => MergeSort.Sort(data);
			public override void Sort<T>(T[] data, IComparer<T> comp) => MergeSort.Sort(data, comp);
			public override void Sort<T>(T[] data, int index, int length) => MergeSort.Sort(data, index, length);
			public override void Sort<T>(T[] data, int index, int length, IComparer<T> comp) => MergeSort.Sort(data, index, length, comp);

			public override void Sort<T>(IList<T> data) => MergeSort.Sort(data);
			public override void Sort<T>(IList<T> data, IComparer<T> comp) => MergeSort.Sort(data, comp);
			public override void Sort<T>(IList<T> data, int index, int length) => MergeSort.Sort(data, index, length);
			public override void Sort<T>(IList<T> data, int index, int length, IComparer<T> comp) => MergeSort.Sort(data, index, length, comp);
		}

		protected override SortAlgorithm Sorter { get; } = new Algorithm();
	}
}