using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Threading;

namespace Kryz.Utils
{
	public class ExactSizeArrayPool<T> : ArrayPool<T>
	{
		public static new readonly ExactSizeArrayPool<T> Shared = new();

		private readonly ConcurrentDictionary<int, ConcurrentQueue<T[]>> queues = new();
		private readonly ConcurrentDictionary<int, int> counts = new();
		private readonly T[]?[] fastSlots;
		private readonly int maxPerSize;

		public ExactSizeArrayPool(int fastSlots = 50, int maxPerSize = 50)
		{
			if (fastSlots <= 0) throw new ArgumentOutOfRangeException(nameof(fastSlots));
			if (maxPerSize <= 0) throw new ArgumentOutOfRangeException(nameof(maxPerSize));

			this.maxPerSize = maxPerSize;
			this.fastSlots = new T[fastSlots][];
		}

		public override T[] Rent(int length)
		{
			if (length < 0) throw new ArgumentOutOfRangeException(nameof(length));

			if (length < fastSlots.Length)
			{
				T[]? slot = fastSlots[length];

				if (slot != null && Interlocked.CompareExchange(ref fastSlots[length], null, slot) == slot)
				{
					return slot;
				}
			}

			if (queues.TryGetValue(length, out ConcurrentQueue<T[]> queue))
			{
				if (queue.TryDequeue(out T[] arr))
				{
					counts.AddOrUpdate(length, 0, (_, count) => Math.Max(0, count - 1));
					return arr;
				}
			}

			return new T[length];
		}

		public override void Return(T[] array, bool clearArray = false)
		{
			if (array == null) throw new ArgumentNullException(nameof(array));

			int length = array.Length;

			if (clearArray)
			{
				Array.Clear(array, 0, length);
			}

			if (length < fastSlots.Length)
			{
				if (Interlocked.CompareExchange(ref fastSlots[length], array, null) == null)
				{
					return;
				}
			}

			ConcurrentQueue<T[]> queue = queues.GetOrAdd(length, _ => new ConcurrentQueue<T[]>());
			int count = counts.AddOrUpdate(length, 1, (_, count) => count + 1);

			if (count <= maxPerSize)
			{
				queue.Enqueue(array);
			}
			else
			{
				counts.AddOrUpdate(length, 0, (_, count) => Math.Max(0, count - 1));
			}
		}
	}
}