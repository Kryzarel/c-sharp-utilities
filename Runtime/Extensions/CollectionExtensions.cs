using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Kryz.Utils
{
	public static class CollectionExtensions
	{
		public static bool TryGetNewCapacity(this int currentCapacity, int desiredCapacity, out int newCapacity)
		{
			if (desiredCapacity > currentCapacity)
			{
				newCapacity = (int)Math.Min(int.MaxValue, (uint)currentCapacity * 2);
				newCapacity = Math.Max(newCapacity, desiredCapacity);
				return true;
			}
			newCapacity = currentCapacity;
			return false;
		}

		public static void AddRangeNonAlloc<T>(this IList<T> list, IList<T> toAdd)
		{
			int count = toAdd.Count;
			for (int i = 0; i < count; i++)
			{
				list.Add(toAdd[i]);
			}
		}

		public static void AddRangeNonAlloc<T>(this IList<T> list, IReadOnlyList<T> toAdd)
		{
			int count = toAdd.Count;
			for (int i = 0; i < count; i++)
			{
				list.Add(toAdd[i]);
			}
		}

		public static void AddRangeNonAlloc<T>(this IList<T> list, ReadOnlySpan<T> toAdd)
		{
			int count = toAdd.Length;
			for (int i = 0; i < count; i++)
			{
				list.Add(toAdd[i]);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool ContentEquals<T>(this IReadOnlyList<T> a, IReadOnlyList<T> b)
		{
			return a.ContentEquals<T, IReadOnlyList<T>, IReadOnlyList<T>, EqualityComparer<T>>(b, EqualityComparer<T>.Default);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool ContentEquals<T, TComp>(this IReadOnlyList<T> a, IReadOnlyList<T> b, TComp comparer) where TComp : IEqualityComparer<T>
		{
			return a.ContentEquals<T, IReadOnlyList<T>, IReadOnlyList<T>, TComp>(b, comparer);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool ContentEquals<T, TList>(this TList a, TList b) where TList : IReadOnlyList<T>
		{
			return a.ContentEquals<T, TList, TList, EqualityComparer<T>>(b, EqualityComparer<T>.Default);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool ContentEquals<T, TList, TComp>(this TList a, TList b, TComp comparer) where TList : IReadOnlyList<T> where TComp : IEqualityComparer<T>
		{
			return a.ContentEquals<T, TList, TList, TComp>(b, comparer);
		}

		public static bool ContentEquals<T, TListA, TListB, TComp>(this TListA a, TListB b, TComp comparer) where TListA : IReadOnlyList<T> where TListB : IReadOnlyList<T> where TComp : IEqualityComparer<T>
		{
			int count = a.Count;
			if (count != b.Count)
			{
				return false;
			}

			for (int i = 0; i < count; i++)
			{
				if (!comparer.Equals(a[i], b[i]))
				{
					return false;
				}
			}
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int IndexOf<T, TList>(this TList list, T value) where TList : IReadOnlyList<T>
		{
			return list.IndexOf(value, EqualityComparer<T>.Default);
		}

		public static int IndexOf<T, TList, TComp>(this TList list, T value, TComp comparer) where TList : IReadOnlyList<T> where TComp : IEqualityComparer<T>
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (comparer.Equals(list[i], value))
				{
					return i;
				}
			}
			return -1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Contains<T, TList>(this TList list, T value) where TList : IReadOnlyList<T>
		{
			return list.IndexOf(value) >= 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Contains<T, TList, TComp>(this TList list, TComp comp, T value) where TList : IReadOnlyList<T> where TComp : IEqualityComparer<T>
		{
			return list.IndexOf(value, comp) >= 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T[] ToArray<T>(this ICollection<T> collection)
		{
			return collection.ToArray<T, ICollection<T>>();
		}

		public static T[] ToArray<T, TList>(this TList collection) where TList : ICollection<T>
		{
			if (collection.Count <= 0) return Array.Empty<T>();
			T[] results = new T[collection.Count];
			collection.CopyTo(results, 0);
			return results;
		}
	}
}