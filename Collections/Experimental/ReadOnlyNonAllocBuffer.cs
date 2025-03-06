using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace Kryz.Collections.Experimental
{
	public ref struct ReadOnlyNonAllocBuffer<T>
	{
		private T[]? array;
		private ReadOnlySpan<T> span;
		private readonly ArrayPool<T>? arrayPool;

		public readonly int Count;

		public readonly ReadOnlySpan<T> Span
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => span;
		}

		public readonly T this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => span[index];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ReadOnlyNonAllocBuffer(in ReadOnlySpan<T> span)
		{
			array = null;
			arrayPool = null;
			this.span = span;
			Count = span.Length;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ReadOnlyNonAllocBuffer(in ReadOnlySpan<T> span, T[]? array, ArrayPool<T> arrayPool)
		{
			this.array = array;
			this.arrayPool = arrayPool ?? ArrayPool<T>.Shared;
			this.span = span;
			Count = span.Length;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Dispose()
		{
			if (array != null && arrayPool != null)
			{
				Array.Clear(array, 0, span.Length);
				arrayPool.Return(array);
				array = null;
				span = default;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly ReadOnlySpan<T>.Enumerator GetEnumerator() => span.GetEnumerator();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly T[] ToArray() => span.ToArray();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator ReadOnlySpan<T>(ReadOnlyNonAllocBuffer<T> value) => value.span;
	}
}