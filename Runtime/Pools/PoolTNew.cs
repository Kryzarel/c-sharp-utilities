namespace Kryz.Utils
{
	public class PoolTNew<T> : BasePool<T> where T : new()
	{
		protected override T Create() => new();
		protected override void OnGet(T obj) { }
		protected override void OnReturn(T obj) { }
	}
}