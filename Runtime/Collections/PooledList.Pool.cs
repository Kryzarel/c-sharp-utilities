using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Kryz.Utils
{
	public partial class PooledList<T>
	{
		private static class Pool
		{
			private static readonly ThreadLocal<LocalPool> threadLocalPool = new(() => new LocalPool());

			public static PooledList<T> Rent(int capacity = 0, ArrayPool<T>? arrayPool = null)
			{
				if (threadLocalPool.Value.TryGet(out PooledList<T>? list))
				{
					return list.Init(capacity, arrayPool);
				}
				return new PooledList<T>(capacity, arrayPool, isPooled: true);
			}

			public static void Return(PooledList<T> list)
			{
				threadLocalPool.Value.Return(list);
			}
		}

		private sealed class LocalPool
		{
			// Fixed size array for pooled lists. Limits memory usage and avoids resizes.
			private readonly PooledList<T>[] pool = new PooledList<T>[128];
			private int count;

			public bool TryGet([MaybeNullWhen(returnValue: false)] out PooledList<T> list)
			{
				if (count > 0)
				{
					list = pool[--count];
					pool[count] = null!;
					return true;
				}
				list = null;
				return false;
			}

			public void Return(PooledList<T> list)
			{
				if (count < pool.Length)
				{
					pool[count++] = list;
				}
			}
		}
	}
}