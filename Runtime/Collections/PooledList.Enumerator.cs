using System;
using System.Collections;
using System.Collections.Generic;

namespace Kryz.Utils
{
	public partial class PooledList<T>
	{
		public struct Enumerator : IEnumerator<T>
		{
			private readonly PooledList<T> list;
			private readonly int version;

			private int index;
			private T current;

			public readonly T Current => current;
			readonly object? IEnumerator.Current => current;

			public Enumerator(PooledList<T> list)
			{
				this.list = list;
				version = list.version;
				index = 0;
				current = default!;
			}

			public bool MoveNext()
			{
				if (version != list.version)
				{
					throw new InvalidOperationException("Collection was modified; enumeration operation may not execute");
				}

				if (index < list.count)
				{
					current = list.array[index++];
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
		}
	}
}