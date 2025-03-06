using System;
using System.Linq;
using NUnit.Framework;
using Random = UnityEngine.Random;

namespace Kryz.Collections.Experimental.Tests.Editor
{
	public class NonAllocBufferTests
	{
		[Test]
		// Given, When, Then
		public void EmptySpan_SetCount_ThrowsException()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() =>
			{
				// Arrange
				using NonAllocBuffer<int> ints = new(stackalloc int[0]);
				// Act
				ints.SetCount(10);
			}); // Assert
		}

		[Test]
		// Given, When, Then
		public void EmptyArray_SetCount_ThrowsException()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() =>
			{
				// Arrange
				using NonAllocBuffer<int> ints = new(0);
				// Act
				ints.SetCount(10);
			}); // Assert
		}

		[Test]
		// Given, When, Then
		public void EmptyArray_EnsureCapacity_MatchesCapacity([Values(1, 10, 100, 1000)] int capacity)
		{
			// Arrange
			using NonAllocBuffer<int> ints = new(stackalloc int[0]);
			// Act
			ints.EnsureCapacity(capacity);
			// Assert
			Assert.GreaterOrEqual(ints.Capacity, capacity);
		}

		delegate T BufferAccessor<T, TBuffer>(in NonAllocBuffer<TBuffer> buffer);
		private static readonly BufferAccessor<int[]?, int> getArray = RefStructReflection.CreateAccessor<BufferAccessor<int[]?, int>>("array");

		[Test]
		// Given, When, Then
		public void InitWithSpan_NoItems_ArrayIsNull()
		{
			// Arrange
			using NonAllocBuffer<int> ints = new(stackalloc int[0]);
			// Act
			int[]? array = getArray(ints);
			// Assert
			Assert.IsNull(array);
		}

		[Test]
		// Given, When, Then
		public void InitWithCapacity_NoItems_ArrayIsNotNull()
		{
			// Arrange
			using NonAllocBuffer<int> ints = new(0);
			// Act
			int[]? array = getArray(ints);
			// Assert
			Assert.IsNotNull(array);
		}

		[Test]
		// Given, When, Then
		public void InitWithSpan_AddItems_CheckIfArrayIsNull([Values(1, 10, 100, 1000)] int capacity, [Values(1, 10, 100, 1000)] int itemsCount, [Values(false, true)] bool useAddRange)
		{
			// Arrange
			using NonAllocBuffer<int> ints = new(stackalloc int[capacity]);

			// Act
			if (useAddRange)
			{
				ints.AddRange(Enumerable.Range(0, itemsCount).ToArray());
			}
			else
			{
				foreach (int item in Enumerable.Range(0, itemsCount))
				{
					ints.Add(item);
				}
			}

			// Assert
			if (itemsCount > capacity)
			{
				Assert.IsNotNull(getArray(ints));
			}
			else
			{
				Assert.IsNull(getArray(ints));
			}
		}

		[Test]
		// Given, When, Then
		public void InitWithCapacity_AddItems_ArrayIsNotNull([Values(1, 10, 100, 1000)] int capacity, [Values(1, 10, 100, 1000)] int itemsCount, [Values(false, true)] bool useAddRange)
		{
			// Arrange
			using NonAllocBuffer<int> ints = new(capacity);

			// Act
			if (useAddRange)
			{
				ints.AddRange(Enumerable.Range(0, itemsCount).ToArray());
			}
			else
			{
				foreach (int item in Enumerable.Range(0, itemsCount))
				{
					ints.Add(item);
				}
			}

			// Assert
			Assert.IsNotNull(getArray(ints));
		}

		[Test]
		// Given, When, Then
		public void InitWithSpan_AddItems_CheckCount([Values(1, 10, 100, 1000)] int capacity, [Values(1, 10, 100, 1000)] int itemsCount, [Values(false, true)] bool useAddRange)
		{
			// Arrange
			using NonAllocBuffer<int> ints = new(stackalloc int[capacity]);

			// Act
			if (useAddRange)
			{
				ints.AddRange(Enumerable.Range(0, itemsCount).ToArray());
			}
			else
			{
				foreach (int item in Enumerable.Range(0, itemsCount))
				{
					ints.Add(item);
				}
			}

			// Assert
			Assert.AreEqual(itemsCount, ints.Count, delta: 0);
		}

		[Test]
		// Given, When, Then
		public void InitWithCapacity_AddItems_CheckCount([Values(1, 10, 100, 1000)] int capacity, [Values(1, 10, 100, 1000)] int itemsCount, [Values(false, true)] bool useAddRange)
		{
			// Arrange
			using NonAllocBuffer<int> ints = new(capacity);

			// Act
			if (useAddRange)
			{
				ints.AddRange(Enumerable.Range(0, itemsCount).ToArray());
			}
			else
			{
				foreach (int item in Enumerable.Range(0, itemsCount))
				{
					ints.Add(item);
				}
			}

			// Assert
			Assert.AreEqual(itemsCount, ints.Count, delta: 0);
		}

		[Test]
		// Given, When, Then
		public void InitWithSpan_AddItems_CheckItems([Values(1, 10, 100, 1000)] int capacity, [Values(1, 10, 100, 1000)] int itemsCount, [Values(false, true)] bool useAddRange)
		{
			// Arrange
			using NonAllocBuffer<int> ints = new(stackalloc int[capacity]);
			Span<int> values = stackalloc int[itemsCount];
			for (int i = 0; i < values.Length; i++)
			{
				values[i] = Random.Range(int.MinValue, int.MaxValue);
			}

			// Act
			if (useAddRange)
			{
				ints.AddRange(values);
			}
			else
			{
				foreach (int item in values)
				{
					ints.Add(item);
				}
			}

			// Assert
			for (int i = 0; i < ints.Count; i++)
			{
				Assert.AreEqual(values[i], ints[i], delta: 0);
			}
		}

		[Test]
		// Given, When, Then
		public void InitWithCapacity_AddItems_CheckItems([Values(1, 10, 100, 1000)] int capacity, [Values(1, 10, 100, 1000)] int itemsCount, [Values(false, true)] bool useAddRange)
		{
			// Arrange
			using NonAllocBuffer<int> ints = new(capacity);
			Span<int> values = new int[itemsCount];
			for (int i = 0; i < values.Length; i++)
			{
				values[i] = Random.Range(int.MinValue, int.MaxValue);
			}

			// Act
			if (useAddRange)
			{
				ints.AddRange(values);
			}
			else
			{
				foreach (int item in values)
				{
					ints.Add(item);
				}
			}

			// Assert
			for (int i = 0; i < ints.Count; i++)
			{
				Assert.AreEqual(values[i], ints[i], delta: 0);
			}
		}

		[Test]
		// Given, When, Then
		public void InitWithSpan_AddItems_Clear_CountIsZero([Values(1, 10, 100, 1000)] int capacity, [Values(1, 10, 100, 1000)] int itemsCount, [Values(false, true)] bool useAddRange)
		{
			// Arrange
			using NonAllocBuffer<int> ints = new(stackalloc int[capacity]);
			Span<int> values = stackalloc int[itemsCount];
			for (int i = 0; i < values.Length; i++)
			{
				values[i] = Random.Range(int.MinValue, int.MaxValue);
			}

			// Act
			if (useAddRange)
			{
				ints.AddRange(values);
			}
			else
			{
				foreach (int item in values)
				{
					ints.Add(item);
				}
			}

			ints.Clear();

			// Assert
			Assert.AreEqual(0, ints.Count, delta: 0);
		}

		[Test]
		// Given, When, Then
		public void InitWithCapacity_AddItems_Clear_CountIsZero([Values(1, 10, 100, 1000)] int capacity, [Values(1, 10, 100, 1000)] int itemsCount, [Values(false, true)] bool useAddRange)
		{
			// Arrange
			using NonAllocBuffer<int> ints = new(capacity);
			Span<int> values = new int[itemsCount];
			for (int i = 0; i < values.Length; i++)
			{
				values[i] = Random.Range(int.MinValue, int.MaxValue);
			}

			// Act
			if (useAddRange)
			{
				ints.AddRange(values);
			}
			else
			{
				foreach (int item in values)
				{
					ints.Add(item);
				}
			}

			ints.Clear();

			// Assert
			Assert.AreEqual(0, ints.Count, delta: 0);
		}

		[Test]
		// Given, When, Then
		public void InitWithSpan_AddItems_Dispose_ArrayIsNull([Values(1, 10, 100, 1000)] int capacity, [Values(1, 10, 100, 1000)] int itemsCount, [Values(false, true)] bool useAddRange)
		{
			// Arrange
			using NonAllocBuffer<int> ints = new(stackalloc int[capacity]);
			Span<int> values = stackalloc int[itemsCount];
			for (int i = 0; i < values.Length; i++)
			{
				values[i] = Random.Range(int.MinValue, int.MaxValue);
			}

			// Act
			if (useAddRange)
			{
				ints.AddRange(values);
			}
			else
			{
				foreach (int item in values)
				{
					ints.Add(item);
				}
			}

			ints.Dispose();

			// Assert
			Assert.IsNull(getArray(ints));
		}

		[Test]
		// Given, When, Then
		public void InitWithCapacity_AddItems_Dispose_ArrayIsNull([Values(1, 10, 100, 1000)] int capacity, [Values(1, 10, 100, 1000)] int itemsCount, [Values(false, true)] bool useAddRange)
		{
			// Arrange
			using NonAllocBuffer<int> ints = new(capacity);
			Span<int> values = new int[itemsCount];
			for (int i = 0; i < values.Length; i++)
			{
				values[i] = Random.Range(int.MinValue, int.MaxValue);
			}

			// Act
			if (useAddRange)
			{
				ints.AddRange(values);
			}
			else
			{
				foreach (int item in values)
				{
					ints.Add(item);
				}
			}

			ints.Dispose();

			// Assert
			Assert.IsNull(getArray(ints));
		}
	}
}