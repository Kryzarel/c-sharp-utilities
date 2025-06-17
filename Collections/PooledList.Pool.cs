using System;
using System.Buffers;

namespace Kryz.Collections
{
	public partial class PooledList<T>
	{
		private static class Pool
		{
			private static readonly PooledList<PooledList<T>> pool = new();

			public static PooledList<T> Rent(int capacity = 0, ArrayPool<T>? arrayPool = null)
			{
				lock (pool)
				{
					if (pool.count > 0)
					{
						PooledList<T> list = pool[pool.count - 1];
						pool.RemoveAt(pool.count - 1);
						Init(list, capacity, arrayPool);
						return list;
					}
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

			private static void Init(PooledList<T> list, int capacity, ArrayPool<T>? pool)
			{
				list.arrayPool = pool ?? ArrayPool<T>.Shared;
				list.array = list.arrayPool.Rent(Math.Max(capacity, 16));
				list.count = 0;
				list.version = 0;
			}
		}
	}
}