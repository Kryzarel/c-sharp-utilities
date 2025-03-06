using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Kryz.Utils;

namespace Kryz.Collections
{
	public struct NonAllocList<T> : IList<T>, IReadOnlyList<T>, IDisposable
	{
		private int count;
		private T[] array;
		private readonly ArrayPool<T> arrayPool;

		public readonly bool IsReadOnly => false;

		public readonly int Count
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => count;
		}

		public readonly int Capacity
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => array.Length;
		}

		public readonly T this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => array[index];
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => array[index] = value;
		}

		public NonAllocList(int capacity, ArrayPool<T>? pool = null)
		{
			arrayPool = pool ?? ArrayPool<T>.Shared;
			array = arrayPool.Rent(Math.Max(capacity, 16));
			count = 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(T item)
		{
			if (count == array.Length)
			{
				EnsureCapacity(count + 1);
			}
			array[count++] = item;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Insert(int index, T item)
		{
			if (count == array.Length)
			{
				EnsureCapacity(count + 1);
			}
			if (index < count)
			{
				Array.Copy(array, index, array, index + 1, count - index);
			}
			array[index] = item;
			count++;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Remove(T item)
		{
			int index = IndexOf(item);
			if (index >= 0)
			{
				RemoveAt(index);
				return true;
			}
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void RemoveAt(int index)
		{
			if ((uint)index >= (uint)count)
			{
				throw new ArgumentOutOfRangeException(nameof(index), "Index must be less than the size of the collection");
			}

			count--;
			if (index < count)
			{
				Array.Copy(array, index + 1, array, index, count - index);
			}

			if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
			{
				array[count] = default!;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly int IndexOf(T item)
		{
			return Array.IndexOf(array, item, 0, count);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly int IndexOf(T item, int index)
		{
			return Array.IndexOf(array, item, index, count - index);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Contains(T item)
		{
			return Array.IndexOf(array, item, 0, count) >= 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly void CopyTo(T[] array, int arrayIndex)
		{
			Array.Copy(this.array, 0, array, arrayIndex, count);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly void CopyTo(NonAllocList<T> list)
		{
			Array.Copy(array, 0, list.array, 0, count);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Clear()
		{
			Array.Clear(array, 0, count);
			count = 0;
		}

		public void Dispose()
		{
			Clear();
			arrayPool.Return(array);
			array = null!;
		}

		public void EnsureCapacity(int capacity)
		{
			if (array.Length.TryEnsureCapacity(capacity, out int newCapacity))
			{
				T[] oldArray = array;
				array = arrayPool.Rent(newCapacity);
				Array.Copy(oldArray, array, count);
				Array.Clear(oldArray, 0, count);
				arrayPool.Return(oldArray);
			}
		}

		public readonly Enumerator GetEnumerator() => new(array, count);

		readonly IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
		readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public struct Enumerator : IEnumerator<T>
		{
			private readonly T[] array;
			private readonly int count;

			private int index;
			private T current;

			public readonly T Current => current;
			readonly object? IEnumerator.Current => current;

			public Enumerator(T[] array, int count = 0)
			{
				this.array = array;
				this.count = count > 0 ? count : array.Length;
				index = 0;
				current = default!;
			}

			public bool MoveNext()
			{
				if (index < count)
				{
					current = array[index++];
					return true;
				}
				return false;
			}

			public void Reset()
			{
				index = 0;
				current = default!;
			}

			public readonly void Dispose() { }
		}
	}
}