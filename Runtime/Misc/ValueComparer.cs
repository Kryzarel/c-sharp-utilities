using System;
using System.Collections.Generic;

namespace Kryz.Utils
{
	public readonly struct ValueComparer<T> : IComparer<T> where T : IComparable<T>
	{
		public static readonly ValueComparer<T> Default = new();

		public int Compare(T x, T y) => x.CompareTo(y);
	}
}