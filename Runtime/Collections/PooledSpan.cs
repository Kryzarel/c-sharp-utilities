using System;
using System.Buffers;

namespace Kryz.Utils
{
	/// <summary>
	/// <para>When working with stackalloc, it's common to write code like this:</para>
	/// <para><see cref="Span{T}"/> buffer = size > 1024 ? new T[size] : stackallock T[size]; </para>
	/// <para>But if we want to use <see cref="ArrayPool{T}"/> instead of new T[], we need to keep track of the rented array, clear it and return it to the pool.</para>
	/// <para>With <see cref="PooledSpan{T}"/> we can reduce all of that to a single line, just like with new T[]:</para>
	/// <para><see cref="using"/> <see cref="PooledSpan{T}"/> buffer = size > 1024 ? new <see cref="PooledSpan{T}"/>(size) : stackallock T[size];</para>
	/// <para>Since it implements the Dispose pattern, declaring a <see cref="PooledSpan{T}"/> with the '<see cref="using"/>' keyword, automatically calls Dispose()
	/// when the variable goes out of scope, which clears and returns the rented array.</para>
	/// </summary>
	public readonly ref struct PooledSpan<T>
	{
		public ref T this[int index] => ref Span[index];

		public readonly Span<T> Span;
		public readonly int Length;

		private readonly T[]? array;
		private readonly ArrayPool<T>? pool;

		public PooledSpan(int size, ArrayPool<T>? arrayPool = null)
		{
			pool = arrayPool ?? ArrayPool<T>.Shared;
			array = pool.Rent(size);
			Span = array[..size];
			Length = Span.Length;
		}

		private PooledSpan(Span<T> span)
		{
			Span = span;
			array = null;
			pool = null;
			Length = 0;
		}

		public void Dispose()
		{
			if (array != null && pool != null)
			{
				Array.Clear(array, 0, Span.Length);
				pool.Return(array, clearArray: false);
			}
		}

		public static implicit operator PooledSpan<T>(Span<T> span) => new(span);
		public static implicit operator Span<T>(PooledSpan<T> pooled) => pooled.Span;
	}
}