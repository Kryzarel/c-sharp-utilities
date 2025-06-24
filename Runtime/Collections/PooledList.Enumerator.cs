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
			private readonly int count;
			private readonly int version;

			private int index;
			private T current;

			public readonly T Current => current;
			readonly object? IEnumerator.Current => current;

			public Enumerator(PooledList<T> list)
			{
				this.list = list;
				count = list.count;
				version = list.version;
				index = 0;
				current = default!;
			}

			public bool MoveNext()
			{
				if (version != list.version)
				{
					index = list.count + 1;
					current = default!;
					throw new InvalidOperationException("Collection was modified; enumeration operation may not execute");
				}

				if (index < count)
				{
					current = list[index++];
					return true;
				}
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