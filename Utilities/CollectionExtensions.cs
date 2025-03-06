using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Kryz.Utils
{
	public static class CollectionExtensions
	{
		public static bool TryEnsureCapacity(this int currentCapacity, int desiredCapacity, out int newCapacity)
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

		public static void AddRangeNonAlloc<T, TList, TAdd>(this TList list, TAdd toAdd) where TList : IList<T> where TAdd : IEnumerable<T>
		{
			foreach (T item in toAdd)
			{
				list.Add(item);
			}
		}

		public static void AddRangeWhere<T, TList, TAdd>(this TList list, TAdd toAdd, Func<T, bool> predicate) where TList : IList<T> where TAdd : IEnumerable<T>
		{
			foreach (T item in toAdd)
			{
				if (predicate(item))
				{
					list.Add(item);
				}
			}
		}

		public static void AddRangeNonAlloc<T, TList>(this TList list, IList<T> toAdd) where TList : IList<T>
		{
			int count = toAdd.Count;
			for (int i = 0; i < count; i++)
			{
				list.Add(toAdd[i]);
			}
		}

		public static void AddRangeWhere<T, TList>(this TList list, IList<T> toAdd, Func<T, bool> predicate) where TList : IList<T>
		{
			int count = toAdd.Count;
			for (int i = 0; i < count; i++)
			{
				T item = toAdd[i];
				if (predicate(item))
				{
					list.Add(item);
				}
			}
		}

		public static void AddRangeNonAlloc<T, TList>(this TList list, IReadOnlyList<T> toAdd) where TList : IList<T>
		{
			int count = toAdd.Count;
			for (int i = 0; i < count; i++)
			{
				list.Add(toAdd[i]);
			}
		}

		public static void AddRangeWhere<T, TList>(this TList list, IReadOnlyList<T> toAdd, Func<T, bool> predicate) where TList : IList<T>
		{
			int count = toAdd.Count;
			for (int i = 0; i < count; i++)
			{
				T item = toAdd[i];
				if (predicate(item))
				{
					list.Add(item);
				}
			}
		}

		public static void AddRange<T, TList>(this TList list, Span<T> toAdd) where TList : IList<T>
		{
			int count = toAdd.Length;
			for (int i = 0; i < count; i++)
			{
				list.Add(toAdd[i]);
			}
		}

		public static void AddRangeWhere<T, TList>(this TList list, Span<T> toAdd, Func<T, bool> predicate) where TList : IList<T>
		{
			int count = toAdd.Length;
			for (int i = 0; i < count; i++)
			{
				T item = toAdd[i];
				if (predicate(item))
				{
					list.Add(item);
				}
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
		public static bool ContentEquals<T, TListA, TListB>(this TListA a, TListB b) where TListA : IReadOnlyList<T> where TListB : IReadOnlyList<T>
		{
			return a.ContentEquals<T, TListA, TListB, EqualityComparer<T>>(b, EqualityComparer<T>.Default);
		}

		public static bool ContentEquals<T, TListA, TListB, TComp>(this TListA a, TListB b, TComp comparer) where TListA : IReadOnlyList<T> where TListB : IReadOnlyList<T> where TComp : IEqualityComparer<T>
		{
			if (a.Count != b.Count)
			{
				return false;
			}

			int count = a.Count;
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
		public static int FindIndex<T>(this IReadOnlyList<T> list, Func<T, bool> predicate)
		{
			return list.FindIndex<T, IReadOnlyList<T>>(predicate);
		}

		public static int FindIndex<T, TList>(this TList list, Func<T, bool> predicate) where TList : IReadOnlyList<T>
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (predicate(list[i]))
				{
					return i;
				}
			}
			return -1;
		}

		public static T[] ToArray<T, TList>(this TList list) where TList : IList<T>
		{
			T[] results = new T[list.Count];
			list.CopyTo(results, 0);
			return results;
		}
	}
}