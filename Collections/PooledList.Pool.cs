using System.Buffers;
using System.Diagnostics.CodeAnalysis;

namespace Kryz.Collections
{
	public partial class PooledList<T>
	{
		private static class Pool
		{
			private static readonly PooledList<PooledList<T>> pool = new();

			public static PooledList<T> Rent(int capacity = 0, ArrayPool<T>? arrayPool = null)
			{
				if (TryGetFromPool(out PooledList<T>? list))
				{
					return list.Init(capacity, arrayPool);
				}
				return new PooledList<T>(capacity, arrayPool, isPooled: true);
			}

			public static void Return(PooledList<T> list)
			{
				lock (pool)
				{
					pool.Add(list);
				}
			}

			private static bool TryGetFromPool([MaybeNullWhen(returnValue: false)] out PooledList<T> list)
			{
				lock (pool)
				{
					if (pool.count > 0)
					{
						list = pool[pool.count - 1];
						pool.RemoveAt(pool.count - 1);
						return true;
					}
				}
				list = null;
				return false;
			}
		}
	}
}