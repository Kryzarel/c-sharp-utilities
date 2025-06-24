using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace Kryz.Utils.Experimental
{
	public ref struct NonAllocBuffer<T>
	{
		private int count;
		private T[]? array;
		private Span<T> span;
		private readonly ArrayPool<T> arrayPool;

		public readonly Span<T> Span
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => span;
		}

		public int Count
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get => count;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				if ((uint)value > (uint)span.Length)
					throw new ArgumentOutOfRangeException(nameof(value));
				count = value;
			}
		}

		public readonly int Capacity
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => span.Length;
		}

		public T this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get => span[index];
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => span[index] = value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public NonAllocBuffer(in Span<T> span, int count = 0, ArrayPool<T>? pool = null)
		{
			array = null;
			this.span = span;
			arrayPool = pool ?? ArrayPool<T>.Shared;
			this.count = Math.Min(count, span.Length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public NonAllocBuffer(int minCapacity, int count = 0, ArrayPool<T>? pool = null)
		{
			arrayPool = pool ?? ArrayPool<T>.Shared;
			span = array = arrayPool.Rent(minCapacity);
			this.count = Math.Min(count, span.Length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetCount(int value)
		{
			if ((uint)value > (uint)span.Length)
				throw new ArgumentOutOfRangeException(nameof(value));
			count = value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(T item)
		{
			EnsureCapacity(count + 1);
			span[count++] = item;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AddRange(in ReadOnlySpan<T> values)
		{
			EnsureCapacity(count + values.Length);
			values.CopyTo(span[count..]);
			count += values.Length;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Clear()
		{
			count = 0;
			span.Clear();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsureCapacity(int capacity)
		{
			if (span.Length.TryGetNewCapacity(capacity, out int newCapacity))
			{
				T[]? oldArray = array;
				Span<T> oldSpan = span[..count];
				span = array = arrayPool.Rent(newCapacity);
				oldSpan.CopyTo(span);

				if (oldArray != null)
				{
					oldSpan.Clear();
					arrayPool.Return(oldArray);
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Dispose()
		{
			Clear();

			if (array != null)
			{
				arrayPool.Return(array);
				array = null;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly Span<T>.Enumerator GetEnumerator() => span[..count].GetEnumerator();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly Span<T> GetFilledSpan() => span[..count];
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly T[] ToArray() => span[..count].ToArray();
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly ReadOnlyNonAllocBuffer<T> AsReadOnly() => new(span[..count], array, arrayPool);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator NonAllocBuffer<T>(Span<T> value) => new(value, value.Length);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Span<T>(NonAllocBuffer<T> value) => value.span;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator ReadOnlySpan<T>(NonAllocBuffer<T> value) => value.span[..value.count];
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator ReadOnlyNonAllocBuffer<T>(NonAllocBuffer<T> value) => value.AsReadOnly();
	}
}