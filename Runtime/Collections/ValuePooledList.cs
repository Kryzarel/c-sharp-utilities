using System;
using System.Buffers;

namespace Kryz.Utils
{
	public ref struct ValuePooledList<T>
	{
		private ArrayPool<T>? arrayPool;

		private Span<T> span;
		private T[]? array;
		private int count;

		public readonly int Count => count;
		public readonly int Capacity => span.Length;

		public readonly Span<T> AsSpan() => span[..count];

		public ValuePooledList(Span<T> initialSpan)
		{
			arrayPool = ArrayPool<T>.Shared;
			span = initialSpan;
			array = null;
			count = 0;
		}

		public ValuePooledList(int capacity, ArrayPool<T>? pool = null)
		{
			arrayPool = pool ?? ArrayPool<T>.Shared;
			array = arrayPool.Rent(capacity);
			span = array;
			count = 0;
		}

		public void Add(T item)
		{
			if (count == span.Length)
				Grow();

			span[count++] = item;
		}

		public ref T Add()
		{
			if (count == span.Length)
				Grow();

			return ref span[count++];
		}

		public void AddRange(ReadOnlySpan<T> items)
		{
			if (count + items.Length > span.Length)
				Grow(count + items.Length);

			items.CopyTo(span[count..]);
			count += items.Length;
		}

		public void Clear()
		{
			count = 0;
		}

		private void Grow(int requiredCapacity = 0)
		{
			int newCapacity = Math.Max(requiredCapacity, span.Length == 0 ? 4 : span.Length * 2);

			T[]? oldArray = array;

			arrayPool ??= ArrayPool<T>.Shared;
			array = arrayPool.Rent(newCapacity);
			span[..count].CopyTo(array);
			span = array;

			if (oldArray != null)
			{
				Array.Clear(oldArray, 0, count);
				arrayPool.Return(oldArray);
			}
		}

		public void Dispose()
		{
			if (array != null)
			{
				Array.Clear(array, 0, count);
				arrayPool ??= ArrayPool<T>.Shared;
				arrayPool.Return(array);
				array = null;
			}
		}

		public static implicit operator Span<T>(ValuePooledList<T> pooled) => pooled.AsSpan();
		public static implicit operator ReadOnlySpan<T>(ValuePooledList<T> pooled) => pooled.AsSpan();
	}
}