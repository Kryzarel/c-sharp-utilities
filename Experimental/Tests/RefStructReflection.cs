using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Kryz.Utils.Experimental.Tests.Editor
{
	/// <summary>
	/// Taken from: https://stackoverflow.com/a/55879736
	/// </summary>
	public static class RefStructReflection
	{
		public static TDelegate CreateAccessor<TDelegate>(string memberName) where TDelegate : Delegate
		{
			MethodInfo invokeMethod = typeof(TDelegate).GetMethod("Invoke");
			if (invokeMethod == null)
			{
				throw new InvalidOperationException($"{typeof(TDelegate)} signature could not be determined.");
			}

			ParameterInfo[] delegateParameters = invokeMethod.GetParameters();
			if (delegateParameters.Length != 1)
			{
				throw new InvalidOperationException("Delegate must have a single parameter.");
			}

			ParameterExpression objParam = Expression.Parameter(delegateParameters[0].ParameterType, "obj");
			MemberExpression memberExpr = Expression.PropertyOrField(objParam, memberName);
			Expression returnExpr = memberExpr;
			if (invokeMethod.ReturnType != memberExpr.Type)
			{
				returnExpr = Expression.ConvertChecked(memberExpr, invokeMethod.ReturnType);
			}

			Expression<TDelegate> lambda = Expression.Lambda<TDelegate>(returnExpr, $"Access{delegateParameters[0].ParameterType.Name}_{memberName}", new[] { objParam });
			return lambda.Compile();
		}
	}
}