using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Kryz.Utils;

namespace Kryz.Collections
{
	public partial class PooledList<T> : IList<T>, IReadOnlyList<T>, IDisposable
	{
		private int version;
		private int count;
		private T[] array;
		private ArrayPool<T> arrayPool;
		private readonly bool isPooled;

		public bool IsReadOnly => false;

		public int Count
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => count;
		}

		public int Capacity
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => array.Length;
		}

		public T this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => array[index];
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => array[index] = value;
		}

		public static PooledList<T> Rent(int capacity = 0, ArrayPool<T>? arrayPool = null)
		{
			return Pool.Rent(capacity, arrayPool);
		}

		public PooledList(int capacity = 0, ArrayPool<T>? arrayPool = null)
		{
			this.arrayPool = arrayPool ?? ArrayPool<T>.Shared;
			array = this.arrayPool.Rent(Math.Max(capacity, 16));
			count = 0;
			version = 0;
		}

		private PooledList(int capacity, ArrayPool<T>? arrayPool, bool isPooled) : this(capacity, arrayPool)
		{
			this.isPooled = isPooled;
		}

		/// <summary>
		/// Used instead of constructor when renting from the static pool. Make sure to keep it equal to constructor, if that ever changes.
		/// </summary>
		private PooledList<T> Init(int capacity = 0, ArrayPool<T>? arrayPool = null)
		{
			this.arrayPool = arrayPool ?? ArrayPool<T>.Shared;
			array = this.arrayPool.Rent(Math.Max(capacity, 16));
			count = 0;
			version = 0;
			return this;
		}

		~PooledList()
		{
			Dispose();
		}

		public void Add(T item)
		{
			if (count == array.Length)
			{
				EnsureCapacity(count + 1);
			}
			array[count++] = item;
			version++;
		}

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
			version++;
		}

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

		public void RemoveAt(int index)
		{
			if ((uint)index >= (uint)count)
			{
				throw new ArgumentOutOfRangeException(nameof(index), "Index must be less than the size of the collection");
			}

			count--;
			version++;

			if (index < count)
			{
				Array.Copy(array, index + 1, array, index, count - index);
			}

			if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
			{
				array[count] = default!;
			}
		}

		public void RemoveRange(int index, int count)
		{
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(index), "Index must be a positive number");
			}

			if (count < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(count), "Count must be a positive number");
			}

			if (this.count - index < count)
			{
				throw new ArgumentException("Invalid offset length");
			}

			if (count > 0)
			{
				this.count -= count;
				if (index < this.count)
				{
					Array.Copy(array, index + count, array, index, this.count - index);
				}

				version++;
				if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
				{
					Array.Clear(array, this.count, count);
				}
			}
		}

		public int RemoveAll<TEquatable>(TEquatable match) where TEquatable : IEquatable<T>
		{
			int freeIndex = 0; // the first free slot in the array

			// Find the first item which needs to be removed.
			while (freeIndex < count && !match.Equals(array[freeIndex]))
			{
				freeIndex++;
			}
			if (freeIndex >= count) return 0;

			int current = freeIndex + 1;
			while (current < count)
			{
				// Find the first item which needs to be kept.
				while (current < count && match.Equals(array[current]))
				{
					current++;
				}

				if (current < count)
				{
					// copy item to the free slot.
					array[freeIndex++] = array[current++];
				}
			}

			if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
			{
				Array.Clear(array, freeIndex, count - freeIndex); // Clear the elements so that the gc can reclaim the references.
			}

			int result = count - freeIndex;
			count = freeIndex;
			version++;
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int IndexOf(T item) => Array.IndexOf(array, item, 0, count);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int IndexOf(T item, int index) => Array.IndexOf(array, item, index, count - index);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Contains(T item) => Array.IndexOf(array, item, 0, count) >= 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyTo(T[] array, int arrayIndex) => Array.Copy(this.array, 0, array, arrayIndex, count);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyTo(PooledList<T> list) => Array.Copy(array, 0, list.array, 0, count);

		public void Clear()
		{
			if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
			{
				Array.Clear(array, 0, count);
			}
			count = 0;
			version++;
		}

		public void Dispose()
		{
			if (array == null || arrayPool == null)
			{
				return;
			}

			Clear();
			arrayPool.Return(array);
			array = null!;
			arrayPool = null!;

			if (isPooled)
			{
				Pool.Return(this);
			}
		}

		public void EnsureCapacity(int capacity)
		{
			if (array.Length.TryGetNewCapacity(capacity, out int newCapacity))
			{
				T[] oldArray = array;
				array = arrayPool.Rent(newCapacity);
				Array.Copy(oldArray, array, count);
				if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
				{
					Array.Clear(oldArray, 0, count);
				}
				arrayPool.Return(oldArray);
			}
		}

		public Enumerator GetEnumerator() => new(this);

		IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}