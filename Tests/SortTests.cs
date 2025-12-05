using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kryz.Utils.Tests
{
	public abstract class SortTests
	{
		protected abstract class SortAlgorithm
		{
			public abstract void Sort<T>(T[] data);
			public abstract void Sort<T>(T[] data, IComparer<T> comp);
			public abstract void Sort<T>(T[] data, int index, int length);
			public abstract void Sort<T>(T[] data, int index, int length, IComparer<T> comp);

			public abstract void Sort<T>(IList<T> data);
			public abstract void Sort<T>(IList<T> data, IComparer<T> comp);
			public abstract void Sort<T>(IList<T> data, int index, int length);
			public abstract void Sort<T>(IList<T> data, int index, int length, IComparer<T> comp);
		}

		protected abstract SortAlgorithm Sorter { get; }

		private static readonly int[] expected = Enumerable.Range(0, 500).ToArray();
		private static readonly int[] shuffled = Shuffle((int[])expected.Clone());

		public static T[] Shuffle<T>(T[] array)
		{
			Random random = new(12345);
			int n = array.Length;
			while (n > 1)
			{
				int k = random.Next(0, n--);
				(array[k], array[n]) = (array[n], array[k]);
			}
			return array;
		}

		private static bool IsSorted<T>(IList<T> data) => IsSorted(data, 0, data.Count, Comparer<T>.Default);
		private static bool IsSorted<T>(IList<T> data, IComparer<T> comparer) => IsSorted(data, 0, data.Count, comparer);
		private static bool IsSorted<T>(IList<T> data, int index, int length) => IsSorted(data, index, length, Comparer<T>.Default);
		private static bool IsSorted<T>(IList<T> data, int index, int length, IComparer<T> comparer)
		{
			for (int i = index + 1; i < index + length; i++)
			{
				if (comparer.Compare(data[i - 1], data[i]) > 0)
				{
					return false;
				}
			}
			return true;
		}

		[Test]
		public void Sort_Array_Full()
		{
			// Arrange
			int[] data = (int[])shuffled.Clone();

			// Act
			Sorter.Sort(data);

			// Assert
			CollectionAssert.AreEqual(expected, data);
			Assert.IsTrue(IsSorted(data));
		}

		[Test]
		public void Sort_Array_Subrange([Values(5, 10)] int index, [Values(5, 10)] int length)
		{
			// Arrange
			int[] data = (int[])shuffled.Clone();
			int[] expected = (int[])shuffled.Clone();

			// Act
			Sorter.Sort(data, index, length);
			Array.Sort(expected, index, length);

			// Assert
			int end = index + length;
			CollectionAssert.AreEqual(expected[index..end], data[index..end]);
			Assert.IsTrue(IsSorted(data, index, length));
		}

		[Test]
		public void Sort_List_Full()
		{
			// Arrange
			IList<int> data = (int[])shuffled.Clone();

			// Act
			Sorter.Sort(data);

			// Assert
			CollectionAssert.AreEqual(expected, data);
			Assert.IsTrue(IsSorted(data));
		}

		[Test]
		public void Sort_List_Subrange([Values(5, 10)] int index, [Values(5, 10)] int length)
		{
			// Arrange
			int[] data = (int[])shuffled.Clone();
			int[] expected = (int[])shuffled.Clone();

			// Act
			// Make sure we use the list variant and not array
			Sorter.Sort((IList<int>)data, index, length);
			Array.Sort(expected, index, length);

			// Assert
			int end = index + length;
			CollectionAssert.AreEqual(expected[index..end], data[index..end]);
			Assert.IsTrue(IsSorted(data, index, length));
		}

		[Test]
		public void Sort_IsStable()
		{
			// Arrange
			(int, int)[] array = new (int, int)[] {
				new(5, 0),
				new(1, 1),
				new(5, 2),
				new(3, 3),
				new(5, 4)
			};

			List<(int, int)> list = new(array);
			Comparer<(int, int)> comp = Comparer<(int, int)>.Create((a, b) => a.Item1.CompareTo(b.Item1));

			// Act
			Sorter.Sort(array, comp);
			Sorter.Sort(list, comp);

			// Assert
			List<int> key5_array = new();
			List<int> key5_list = new();

			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Item1 == 5) key5_array.Add(array[i].Item2);
				if (list[i].Item1 == 5) key5_list.Add(list[i].Item2);
			}

			Assert.IsTrue(IsSorted(array, comp));
			Assert.IsTrue(IsSorted(list, comp));

			// They must remain in original order: [0, 2, 4]
			CollectionAssert.AreEqual(new[] { 0, 2, 4 }, key5_array);
			CollectionAssert.AreEqual(new[] { 0, 2, 4 }, key5_list);
		}

		[Test]
		public void Sort_EmptyArray()
		{
			int[] arr = Array.Empty<int>();
			Sorter.Sort(arr);
			Assert.IsTrue(IsSorted(arr));
		}

		[Test]
		public void Sort_SingleElement()
		{
			int[] arr = { 42 };
			Sorter.Sort(arr);
			Assert.AreEqual(new[] { 42 }, arr);
		}
	}
}