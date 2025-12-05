using System.Collections.Generic;

namespace Kryz.Utils.Tests
{
	public class MergeBinarySortTests : SortTests
	{
		protected class Algorithm : SortAlgorithm
		{
			public override void Sort<T>(T[] data) => MergeBinarySort.Sort(data);
			public override void Sort<T>(T[] data, IComparer<T> comp) => MergeBinarySort.Sort(data, comp);
			public override void Sort<T>(T[] data, int index, int length) => MergeBinarySort.Sort(data, index, length);
			public override void Sort<T>(T[] data, int index, int length, IComparer<T> comp) => MergeBinarySort.Sort(data, index, length, comp);

			public override void Sort<T>(IList<T> data) => MergeBinarySort.Sort(data);
			public override void Sort<T>(IList<T> data, IComparer<T> comp) => MergeBinarySort.Sort(data, comp);
			public override void Sort<T>(IList<T> data, int index, int length) => MergeBinarySort.Sort(data, index, length);
			public override void Sort<T>(IList<T> data, int index, int length, IComparer<T> comp) => MergeBinarySort.Sort(data, index, length, comp);
		}

		protected override SortAlgorithm Sorter { get; } = new Algorithm();
	}
}