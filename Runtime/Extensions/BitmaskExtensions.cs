namespace Kryz.Utils
{
	public static class BitmaskExtensions
	{
		public static bool HasFlag(this byte mask, byte flag)
		{
			return (mask & flag) != 0;
		}

		public static bool HasFlag(this short mask, short flag)
		{
			return (mask & flag) != 0;
		}

		public static bool HasFlag(this ushort mask, ushort flag)
		{
			return (mask & flag) != 0;
		}

		public static bool HasFlag(this int mask, int flag)
		{
			return (mask & flag) != 0;
		}

		public static bool HasFlag(this uint mask, uint flag)
		{
			return (mask & flag) != 0;
		}

		public static bool HasFlag(this long mask, long flag)
		{
			return (mask & flag) != 0;
		}

		public static bool HasFlag(this ulong mask, ulong flag)
		{
			return (mask & flag) != 0;
		}
	}
}