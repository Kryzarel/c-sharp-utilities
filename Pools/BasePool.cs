using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace Kryz.Pools
{
	public abstract class BasePool<T>
	{
		private int maxSize;
		private int count;
		private T[] array;
		private readonly ArrayPool<T> arrayPool;

		public int MaxSize
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => maxSize;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => maxSize = value > 0 ? value : int.MaxValue;
		}

		public int Count
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => count;
		}

		public int Capacity
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => array.Length;
		}

		public BasePool(int maxSize = 0, int capacity = 0, ArrayPool<T>? arrayPool = null)
		{
			this.maxSize = maxSize > 0 ? maxSize : int.MaxValue;
			this.arrayPool = arrayPool ?? ArrayPool<T>.Shared;
			array = this.arrayPool.Rent(Math.Max(capacity, 16));
			count = 0;
		}

		public T Get()
		{
			T item = count > 0 ? array[--count] : Create();
			OnGet(item);
			return item;
		}

		public void Return(T item)
		{
			OnReturn(item);
			if (count >= maxSize)
			{
				return;
			}
			if (count == array.Length)
			{
				EnsureCapacity(count + 1);
			}
			array[count++] = item;
		}

		public void Fill(int desiredCount)
		{
			int targetCount = Math.Min(desiredCount, maxSize);
			EnsureCapacity(targetCount);

			while (count < targetCount)
			{
				Return(Create());
			}
		}

		private void EnsureCapacity(int capacity)
		{
			if (Utils.CollectionExtensions.TryEnsureCapacity(array.Length, capacity, out int newCapacity))
			{
				T[] oldArray = array;
				array = arrayPool.Rent(newCapacity);
				Array.Copy(oldArray, array, count);
				Array.Clear(oldArray, 0, count);
				arrayPool.Return(oldArray);
			}
		}

		protected abstract T Create();
		protected abstract void OnGet(T obj);
		protected abstract void OnReturn(T obj);
	}
}