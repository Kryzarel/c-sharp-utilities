using System;
using System.Buffers;

namespace Kryz.Utils
{
	/// <summary>
	/// <para>When working with stackalloc, it's common to write code like this:</para>
	/// <para><see cref="Span{T}"/> buffer = size > 1024 ? new T[size] : stackallock T[size]; </para>
	/// <para>But if we want to use <see cref="ArrayPool{T}"/> instead of new T[], we need to keep track of the rented array, clear it and return it to the pool.</para>
	/// <para>With SpanScope we can reduce all of that to a single line, just like with new T[]:</para>
	/// <para><see cref="using"/> <see cref="SpanScope{T}"/> scope = size > 1024 ? new <see cref="SpanScope{T}"/>(size) : stackallock T[size];</para>
	/// <para>Since it implements the Dispose pattern, declaring a <see cref="SpanScope{T}"/> variable with the '<see cref="using"/>' keyword causes the Dispose()
	/// method to be automatically called when the variable goes out of scope, which clears and returns the array.</para>
	/// </summary>
	public readonly ref struct SpanScope<T>
	{
		public readonly Span<T> Span;

		private readonly T[]? array;
		private readonly ArrayPool<T>? pool;

		public SpanScope(int size, ArrayPool<T>? arrayPool = null)
		{
			pool = arrayPool ?? ArrayPool<T>.Shared;
			array = pool.Rent(size);
			Span = array[..size];
		}

		private SpanScope(Span<T> span)
		{
			Span = span;
			array = null;
			pool = null;
		}

		public void Dispose()
		{
			if (array != null && pool != null)
			{
				Array.Clear(array, 0, Span.Length);
				pool.Return(array, clearArray: false);
			}
		}

		public static implicit operator SpanScope<T>(Span<T> span) => new(span);
		public static implicit operator Span<T>(SpanScope<T> arrayPoolScope) => arrayPoolScope.Span;
	}
}