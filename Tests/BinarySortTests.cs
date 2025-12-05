using System.Collections.Generic;

namespace Kryz.Utils.Tests
{
	public class BinarySortTests : SortTests
	{
		protected class Algorithm : SortAlgorithm
		{
			public override void Sort<T>(T[] data) => BinarySort.Sort(data);
			public override void Sort<T>(T[] data, IComparer<T> comp) => BinarySort.Sort(data, comp);
			public override void Sort<T>(T[] data, int index, int length) => BinarySort.Sort(data, index, length);
			public override void Sort<T>(T[] data, int index, int length, IComparer<T> comp) => BinarySort.Sort(data, index, length, comp);

			public override void Sort<T>(IList<T> data) => BinarySort.Sort(data);
			public override void Sort<T>(IList<T> data, IComparer<T> comp) => BinarySort.Sort(data, comp);
			public override void Sort<T>(IList<T> data, int index, int length) => BinarySort.Sort(data, index, length);
			public override void Sort<T>(IList<T> data, int index, int length, IComparer<T> comp) => BinarySort.Sort(data, index, length, comp);
		}

		protected override SortAlgorithm Sorter { get; } = new Algorithm();
	}
}