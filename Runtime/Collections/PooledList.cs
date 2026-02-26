using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Kryz.Utils
{
	public partial class PooledList<T> : IList<T>, IReadOnlyList<T>, IDisposable
	{
		private int version;
		private int count;
		private T[] array;
		private ArrayPool<T> pool;
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

		public ref T this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				if ((uint)index >= (uint)count)
					throw new IndexOutOfRangeException("Index was out of range. Must be non-negative and less than the size of the collection.");
				return ref array[index];
			}
		}

		T IList<T>.this[int index]
		{
			get => this[index];
			set => this[index] = value;
		}

		T IReadOnlyList<T>.this[int index] => this[index];

		/// <summary>
		/// Rent a <see cref="PooledList{T}"/> from the pool. In order to return it, call the <see cref="Dispose"/> method.
		/// </summary>
		/// <param name="capacity">The minimum initial size of the internal array used by the <see cref="PooledList{T}"/>. Since this array is obtained from an <see cref="ArrayPool{T}"/>, the actual initial size may be larger than requested.</param>
		/// <param name="arrayPool">The <see cref="ArrayPool{T}"/> object used to obtain the internal arrays. If <see cref="null"/>, <see cref="ArrayPool{T}.Shared"/> is used.</param>
		/// <returns>A <see cref="PooledList{T}"/> from the pool.</returns>
		public static PooledList<T> Rent(int capacity = 0, ArrayPool<T>? arrayPool = null)
		{
			return Pool.Rent(capacity, arrayPool);
		}

		/// <summary>
		/// Create a new <see cref="PooledList{T}"/>, which obtains its internal arrays from an <see cref="ArrayPool{T}"/>. In order to correctly return the internal array to the <see cref="ArrayPool{T}"/>, call the <see cref="Dispose"/> method.
		/// </summary>
		/// <param name="capacity">The minimum initial size of the internal array used by the <see cref="PooledList{T}"/>. Since this array is obtained from an <see cref="ArrayPool{T}"/>, the actual initial size may be larger than requested.</param>
		/// <param name="arrayPool">The <see cref="ArrayPool{T}"/> object used to obtain the internal arrays. If <see cref="null"/>, <see cref="ArrayPool{T}.Shared"/> is used.</param>
		/// <returns>A new <see cref="PooledList{T}"/> object.</returns>
		public PooledList(int capacity = 0, ArrayPool<T>? arrayPool = null)
		{
			Init(capacity, arrayPool, out pool, out array, out count, out version);
		}

		private PooledList(int capacity, ArrayPool<T>? arrayPool, bool isPooled) : this(capacity, arrayPool)
		{
			this.isPooled = isPooled;
		}

		private PooledList<T> Init(int capacity = 0, ArrayPool<T>? arrayPool = null)
		{
			Init(capacity, arrayPool, out pool, out array, out count, out version);
			return this;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void Init(int capacity, ArrayPool<T>? arrayPool, out ArrayPool<T> pool, out T[] array, out int count, out int version)
		{
			pool = arrayPool ?? ArrayPool<T>.Shared;
			array = pool.Rent(capacity > 0 ? capacity : 1);
			count = 0;
			version = 0;
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

		public void AddRange(IEnumerable<T> enumerable)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException(nameof(enumerable));
			}

			if (enumerable is ICollection<T> collection)
			{
				int newCount = count + collection.Count;
				if (newCount > count)
				{
					if (newCount > array.Length)
					{
						EnsureCapacity(newCount);
					}
					collection.CopyTo(array, count);
					count = newCount;
					version++;
				}
			}
			else
			{
				foreach (T item in enumerable)
				{
					Add(item);
				}
			}
		}

		public void AddRange(ReadOnlySpan<T> span)
		{
			int newCount = count + span.Length;
			if (newCount > count)
			{
				if (newCount > array.Length)
				{
					EnsureCapacity(newCount);
				}
				span.CopyTo(array[count..]);
				count = newCount;
				version++;
			}
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
			if (count == 0)
			{
				return false;
			}

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

			array[count] = default!;
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
				Array.Clear(array, this.count, count);
			}
		}

		public int RemoveAll(Predicate<T> match)
		{
			if (match == null) throw new ArgumentNullException(nameof(match));

			int freeIndex = 0; // the first free slot in items array

			// Find the first item which needs to be removed.
			while (freeIndex < count && !match(array[freeIndex])) freeIndex++;
			if (freeIndex >= count) return 0;

			int current = freeIndex + 1;
			while (current < count)
			{
				// Find the first item which needs to be kept.
				while (current < count && match(array[current])) current++;

				if (current < count)
				{
					// copy item to the free slot.
					array[freeIndex++] = array[current++];
				}
			}

			Array.Clear(array, freeIndex, count - freeIndex); // Clear the elements so that the gc can reclaim the references.

			int result = count - freeIndex;
			count = freeIndex;
			version++;
			return result;
		}

		public int RemoveAll<TEquatable>(TEquatable match) where TEquatable : IEquatable<T>
		{
			if (match == null) throw new ArgumentNullException(nameof(match));

			int freeIndex = 0; // the first free slot in items array

			// Find the first item which needs to be removed.
			while (freeIndex < count && !match.Equals(array[freeIndex])) freeIndex++;
			if (freeIndex >= count) return 0;

			int current = freeIndex + 1;
			while (current < count)
			{
				// Find the first item which needs to be kept.
				while (current < count && match.Equals(array[current])) current++;

				if (current < count)
				{
					// copy item to the free slot.
					array[freeIndex++] = array[current++];
				}
			}

			Array.Clear(array, freeIndex, count - freeIndex); // Clear the elements so that the gc can reclaim the references.

			int result = count - freeIndex;
			count = freeIndex;
			version++;
			return result;
		}

		public T[] ToArray()
		{
			if (count <= 0) return Array.Empty<T>();
			T[] result = new T[count];
			Array.Copy(array, 0, result, 0, count);
			return result;
		}

		public Span<T> AsSpan() => new(array, 0, count);

		public void SetCount(int value)
		{
			if (value == count)
				return;

			if (value > count)
			{
				EnsureCapacity(value);
			}
			else
			{
				Array.Clear(array, value, count - value);
			}

			count = value;
			version++;
		}

		public void EnsureCapacity(int capacity)
		{
			if (array.Length < capacity)
			{
				ArrayPoolUtils.Resize(ref array, count, capacity);
			}
		}

		public void Clear()
		{
			Array.Clear(array, 0, count);
			count = 0;
			version++;
		}

		public void Dispose()
		{
			if (array == null || pool == null)
			{
				return;
			}

			Array.Clear(array, 0, count);
			pool.Return(array);
			array = null!;
			pool = null!;

			count = 0;
			version++;

			if (isPooled)
			{
				Pool.Return(this);
			}
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
		public void CopyTo(PooledList<T> list)
		{
			list.EnsureCapacity(count);
			Array.Copy(array, 0, list.array, 0, count);
		}

		public Enumerator GetEnumerator() => new(this);

		IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}