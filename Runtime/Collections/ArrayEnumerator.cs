using System.Collections;
using System.Collections.Generic;

namespace Kryz.Utils
{
	public struct ArrayEnumerator<T> : IEnumerable<T>, IEnumerator<T>
	{
		private readonly T[] array;
		private readonly int count;

		private int index;
		private T current;

		public readonly T Current => current;
		readonly object? IEnumerator.Current => Current;

		public ArrayEnumerator(T[] array) : this(array, array.Length) { }

		public ArrayEnumerator(T[] array, int count)
		{
			this.array = array;
			this.count = count;

			index = 0;
			current = default!;
		}

		public bool MoveNext()
		{
			if (index < count)
			{
				current = array[index++];
				return true;
			}

			index = -1;
			current = default!;
			return false;
		}

		public void Reset()
		{
			index = 0;
			current = default!;
		}

		public readonly void Dispose() { }

		public readonly ArrayEnumerator<T> GetEnumerator() => this;

		readonly IEnumerator<T> IEnumerable<T>.GetEnumerator() => this;
		readonly IEnumerator IEnumerable.GetEnumerator() => this;
	}
}