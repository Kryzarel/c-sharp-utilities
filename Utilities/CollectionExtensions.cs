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

		/// <summary>
		/// <para>Generic version of AddRange, accepting any <see cref="IList{T}"/> as target and any <see cref="IEnumerable{T}"/> as input.</para>
		/// <para>By using generics we can prevent allocations from boxing (i.e. if you pass a struct implementing <see cref="IList{T}"/> or <see cref="IEnumerable{T}"/>, it will NOT be boxed).
		/// And potentially allows the compiler to use concrete methods from the collections, rather than virtual methods, increasing performance.</para>
		/// <para>If the collection implements a value type enumerator, this also prevents allocations when using foreach. If it does not, it is recommended to use the IList or IReadOnlyList versions.</para>
		/// </summary>
		public static void AddRange_IEnumerable<T, TList, TAdd>(this TList list, TAdd toAdd) where TList : IList<T> where TAdd : IEnumerable<T>
		{
			foreach (T item in toAdd)
			{
				list.Add(item);
			}
		}

		/// <summary>
		/// <para>Generic version of AddRange, with a predicate to filter elements to be added, accepting any <see cref="IList{T}"/> as target and any <see cref="IEnumerable{T}"/> as input.</para>
		/// <para>By using generics we can prevent allocations from boxing (i.e. if you pass a struct implementing <see cref="IList{T}"/> or <see cref="IEnumerable{T}"/>, it will NOT be boxed).
		/// And potentially allows the compiler to use concrete methods from the collections, rather than virtual methods, increasing performance.</para>
		/// <para>If the collection implements a value type enumerator, this also prevents allocations when using foreach. If it does not, it is recommended to use the IList or IReadOnlyList versions.</para>
		/// </summary>
		public static void AddRangeWhere_IEnumerable<T, TList, TAdd>(this TList list, TAdd toAdd, Func<T, bool> predicate) where TList : IList<T> where TAdd : IEnumerable<T>
		{
			foreach (T item in toAdd)
			{
				if (predicate(item))
				{
					list.Add(item);
				}
			}
		}

		/// <summary>
		/// <para>Generic version of AddRange, accepting any <see cref="IList{T}"/> as target and any <see cref="IList{T}"/> as input.</para>
		/// <para>By using generics we can prevent allocations from boxing (i.e. if you pass a struct implementing <see cref="IList{T}"/>, it will NOT be boxed).
		/// And potentially allows the compiler to use concrete methods from the collections, rather than virtual methods, increasing performance.</para>
		/// </summary>
		public static void AddRange_IList<T, TList, TAdd>(this TList list, TAdd toAdd) where TList : IList<T> where TAdd : IList<T>
		{
			int count = toAdd.Count;
			for (int i = 0; i < count; i++)
			{
				list.Add(toAdd[i]);
			}
		}

		/// <summary>
		/// <para>Generic version of AddRange, with a predicate to filter elements to be added, accepting any <see cref="IList{T}"/> as target and any <see cref="IList{T}"/> as input.</para>
		/// <para>By using generics we can prevent allocations from boxing (i.e. if you pass a struct implementing <see cref="IList{T}"/>, it will NOT be boxed).
		/// And potentially allows the compiler to use concrete methods from the collections, rather than virtual methods, increasing performance.</para>
		/// </summary>
		public static void AddRangeWhere_IList<T, TList, TAdd>(this TList list, TAdd toAdd, Func<T, bool> predicate) where TList : IList<T> where TAdd : IList<T>
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

		/// <summary>
		/// <para>Generic version of AddRange, accepting any <see cref="IList{T}"/> as target and any <see cref="IReadOnlyList{T}"/> as input.</para>
		/// <para>By using generics we can prevent allocations from boxing (i.e. if you pass a struct implementing <see cref="IList{T}"/> or <see cref="IReadOnlyList{T}"/>, it will NOT be boxed).
		/// And potentially allows the compiler to use concrete methods from the collections, rather than virtual methods, increasing performance.</para>
		/// </summary>
		public static void AddRange_IReadOnlyList<T, TList, TAdd>(this TList list, TAdd toAdd) where TList : IList<T> where TAdd : IReadOnlyList<T>
		{
			int count = toAdd.Count;
			for (int i = 0; i < count; i++)
			{
				list.Add(toAdd[i]);
			}
		}

		/// <summary>
		/// <para>Generic version of AddRange, with a predicate to filter elements to be added, accepting any <see cref="IList{T}"/> as target and any <see cref="IReadOnlyList{T}"/> as input.</para>
		/// <para>By using generics we can prevent allocations from boxing (i.e. if you pass a struct implementing <see cref="IList{T}"/> or <see cref="IReadOnlyList{T}"/>, it will NOT be boxed).
		/// And potentially allows the compiler to use concrete methods from the collections, rather than virtual methods, increasing performance.</para>
		/// </summary>
		public static void AddRangeWhere_IReadOnlyList<T, TList, TAdd>(this TList list, TAdd toAdd, Func<T, bool> predicate) where TList : IList<T> where TAdd : IReadOnlyList<T>
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

		/// <summary>
		/// <para>Generic version of AddRange, with a predicate to filter elements to be added, accepting any <see cref="IList{T}"/> as target and a <see cref="ReadOnlySpan{T}"/> as input.</para>
		/// <para>By using generics we can prevent allocations from boxing (i.e. if you pass a struct implementing <see cref="IList{T}"/>, it will NOT be boxed).
		/// And potentially allows the compiler to use concrete methods from the collections, rather than virtual methods, increasing performance.</para>
		/// </summary>
		public static void AddRange<T, TList>(this TList list, ReadOnlySpan<T> toAdd) where TList : IList<T>
		{
			int count = toAdd.Length;
			for (int i = 0; i < count; i++)
			{
				list.Add(toAdd[i]);
			}
		}

		/// <summary>
		/// <para>Generic version of AddRange, with a predicate to filter elements to be added, accepting any <see cref="IList{T}"/> as target and a <see cref="Span{T}"/> as input.</para>
		/// <para>By using generics we can prevent allocations from boxing (i.e. if you pass a struct implementing <see cref="IList{T}"/>, it will NOT be boxed).
		/// And potentially allows the compiler to use concrete methods from the collections, rather than virtual methods, increasing performance.</para>
		/// </summary>
		public static void AddRangeWhere<T, TList>(this TList list, ReadOnlySpan<T> toAdd, Func<T, bool> predicate) where TList : IList<T>
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
		public static bool ContentEquals<T, TList>(this TList a, TList b) where TList : IReadOnlyList<T>
		{
			return a.ContentEquals<T, TList, TList, EqualityComparer<T>>(b, EqualityComparer<T>.Default);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool ContentEquals<T, TList, TComp>(this TList a, TList b, TComp comp) where TList : IReadOnlyList<T> where TComp : IEqualityComparer<T>
		{
			return a.ContentEquals<T, TList, TList, TComp>(b, comp);
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
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				if (predicate(list[i]))
				{
					return i;
				}
			}
			return -1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T[] ToArray<T>(this ICollection<T> collection)
		{
			return collection.ToArray<T, ICollection<T>>();
		}

		public static T[] ToArray<T, TList>(this TList collection) where TList : ICollection<T>
		{
			T[] results = new T[collection.Count];
			collection.CopyTo(results, 0);
			return results;
		}
	}
}