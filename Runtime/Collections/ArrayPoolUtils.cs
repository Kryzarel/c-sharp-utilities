using System;
using System.Buffers;

namespace Kryz.Utils
{
	public static class ArrayPoolUtils
	{
		public static void Resize<T>(ref T[] array, int oldSize, int newSize) => Resize(ref array, oldSize, newSize, ArrayPool<T>.Shared);

		public static void Resize<T>(ref T[] array, int oldSize, int newSize, ArrayPool<T> arrayPool)
		{
			T[] newArray = arrayPool.Rent(newSize);
			Array.Copy(array, newArray, oldSize);
			Array.Clear(array, 0, oldSize);
			arrayPool.Return(array);
			array = newArray;
		}
	}
}