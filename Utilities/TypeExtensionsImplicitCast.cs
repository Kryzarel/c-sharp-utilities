using System;
using System.Collections.Generic;
using System.Reflection;

namespace Kryz.Utils
{
	public static class TypeExtensionsImplicitCast
	{
		private const BindingFlags flags = BindingFlags.Static | BindingFlags.NonPublic;
		private static readonly MethodInfo assignMethod = typeof(TypeExtensionsImplicitCast).GetMethod(nameof(Assign), flags);
		private static readonly Dictionary<(Type, Type), bool> implicitCastCache = new();
		private static readonly Dictionary<Type, MethodInfo> genericAssignCache = new();
		private static readonly Dictionary<Type, object[]> paramCache = new();

		private static T Assign<T>(T value) => value;

		public static bool IsImplicitlyCastableTo(this Type type, Type target)
		{
			if (type == null || target == null || type == typeof(void) != (target == typeof(void)))
			{
				return false;
			}

			if (type == target || target.IsAssignableFrom(type))
			{
				return true;
			}

			if (!implicitCastCache.TryGetValue((type, target), out bool result))
			{
				if (!genericAssignCache.TryGetValue(target, out MethodInfo method))
				{
					genericAssignCache[target] = method = assignMethod.MakeGenericMethod(target);
				}
				if (!paramCache.TryGetValue(type, out object[] param))
				{
					paramCache[type] = param = new object[] { ObjectCreator.Create(type) };
				}
				implicitCastCache[(type, target)] = result = method.InvokeSafe(null, param);
			}
			return result;
		}

		private static bool InvokeSafe(this MethodInfo methodInfo, object? obj, params object[] param)
		{
			try
			{
				methodInfo.Invoke(obj, param);
				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}