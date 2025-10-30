using System;
using System.Buffers;

namespace Kryz.Utils
{
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