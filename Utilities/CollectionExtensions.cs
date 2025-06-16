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

		/// <summary>
		/// <para>Generic version of AddRange, accepting any <see cref="IList{T}"/> as target and a <see cref="IList{T}"/> as input.</para>
		/// <para>By using generics we can prevent allocations from boxing (i.e. if you pass a struct implementing <see cref="IList{T}"/>, it will NOT be boxed).
		/// And potentially allows the compiler to use concrete methods from the collections, rather than virtual methods, increasing performance.</para>
		/// </summary>
		public static TList AddRangeFromIList<T, TList, TAdd>(this TList list, TAdd toAdd) where TList : IList<T> where TAdd : IList<T>
		{
			int count = toAdd.Count;
			for (int i = 0; i < count; i++)
			{
				list.Add(toAdd[i]);
			}
			return list;
		}

		/// <summary>
		/// <para>Generic version of AddRange, accepting any <see cref="IList{T}"/> as target and a <see cref="IReadOnlyList{T}"/> as input.</para>
		/// <para>By using generics we can prevent allocations from boxing (i.e. if you pass a struct implementing <see cref="IList{T}"/>, it will NOT be boxed).
		/// And potentially allows the compiler to use concrete methods from the collections, rather than virtual methods, increasing performance.</para>
		/// </summary>
		public static TList AddRangeFromIReadOnlyList<T, TList, TAdd>(this TList list, TAdd toAdd) where TList : IList<T> where TAdd : IReadOnlyList<T>
		{
			int count = toAdd.Count;
			for (int i = 0; i < count; i++)
			{
				list.Add(toAdd[i]);
			}
			return list;
		}

		/// <summary>
		/// <para>Generic version of AddRange, accepting any <see cref="IList{T}"/> as target and a <see cref="IEnumerable{T}"/> as input.</para>
		/// <para>By using generics we can prevent allocations from boxing (i.e. if you pass a struct implementing <see cref="IList{T}"/>, it will NOT be boxed).
		/// And potentially allows the compiler to use concrete methods from the collections, rather than virtual methods, increasing performance.</para>
		/// </summary>
		public static TList AddRangeFromIEnumerable<T, TList, TAdd>(this TList list, TAdd toAdd) where TList : IList<T> where TAdd : IEnumerable<T>
		{
			foreach (T value in toAdd)
			{
				list.Add(value);
			}
			return list;
		}

		/// <summary>
		/// <para>Generic version of AddRange, accepting any <see cref="IList{T}"/> as target and a <see cref="ReadOnlySpan{T}"/> as input.</para>
		/// <para>By using generics we can prevent allocations from boxing (i.e. if you pass a struct implementing <see cref="IList{T}"/>, it will NOT be boxed).
		/// And potentially allows the compiler to use concrete methods from the collections, rather than virtual methods, increasing performance.</para>
		/// </summary>
		public static TList AddRangeFromSpan<T, TList>(this TList list, ReadOnlySpan<T> toAdd) where TList : IList<T>
		{
			foreach (T value in toAdd)
			{
				list.Add(value);
			}
			return list;
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
		public static T[] ToArray<T>(this ICollection<T> collection)
		{
			return collection.ToArray<T, ICollection<T>>();
		}

		public static T[] ToArray<T, TList>(this TList collection) where TList : ICollection<T>
		{
			if (collection.Count == 0) return Array.Empty<T>();
			T[] results = new T[collection.Count];
			collection.CopyTo(results, 0);
			return results;
		}
	}
}