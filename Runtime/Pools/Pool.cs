using System;
using System.Buffers;

namespace Kryz.Utils
{
	public sealed class Pool<T> : BasePool<T>
	{
		private readonly Func<T> createFunc;
		private readonly Action<T>? getAction;
		private readonly Action<T>? returnAction;

		public Pool(Func<T> createFunc, Action<T>? getAction = null, Action<T>? returnAction = null, int capacity = 0, int maxSize = 0, ArrayPool<T>? arrayPool = null) : base(capacity, maxSize, arrayPool)
		{
			this.createFunc = createFunc;
			this.getAction = getAction;
			this.returnAction = returnAction;
		}

		protected override T Create() => createFunc();
		protected override void OnGet(T obj) => getAction?.Invoke(obj);
		protected override void OnReturn(T obj) => returnAction?.Invoke(obj);
	}
}