using System;
using System.Collections.Generic;
using System.Reflection;

namespace Kryz.SharpUtils
{
	public static class TypeExtensions
	{
		/// <summary>
		/// C# doesn't give you base class private members even if you use BindingFlags.NonPublic.
		/// We have to manually search the base class(es) to find them.
		/// </summary>
		public static FieldInfo GetFieldInSubclasses(this Type type, string name, BindingFlags bindingFlags)
		{
			// Use BindingFlags.DeclaredOnly to prevent including duplicate fields from base/derived classes
			bindingFlags |= BindingFlags.DeclaredOnly;

			FieldInfo? field;
			do
			{
				field = type.GetField(name, bindingFlags);
				type = type.BaseType;
			}
			while (type != null && field == null);

			return field;
		}

		/// <summary>
		/// C# doesn't give you base class private members even if you use BindingFlags.NonPublic.
		/// We have to manually search the base class(es) to find them.
		/// </summary>
		public static void GetAllFields(this Type type, BindingFlags bindingFlags, List<FieldInfo> fieldInfos)
		{
			// Use BindingFlags.DeclaredOnly to prevent including duplicate fields from base/derived classes
			bindingFlags |= BindingFlags.DeclaredOnly;

			do
			{
				fieldInfos.AddRangeNonAlloc(type.GetFields(bindingFlags));
				type = type.BaseType;
			}
			while (type != null);
		}

		/// <summary>
		/// C# doesn't give you base class private members even if you use BindingFlags.NonPublic.
		/// We have to manually search the base class(es) to find them.
		/// </summary>
		public static void GetAllProperties(this Type type, BindingFlags bindingFlags, List<PropertyInfo> propertyInfos)
		{
			// Use BindingFlags.DeclaredOnly to prevent including duplicate fields from base/derived classes
			bindingFlags |= BindingFlags.DeclaredOnly;

			do
			{
				propertyInfos.AddRangeWhere(type.GetProperties(bindingFlags), item => !item.Exists(propertyInfos));
				type = type.BaseType;
			}
			while (type != null);
		}

		/// <summary>
		/// C# doesn't give you base class private members even if you use BindingFlags.NonPublic.
		/// We have to manually search the base class(es) to find them.
		/// </summary>
		public static void GetAllMethods(this Type type, BindingFlags bindingFlags, List<MethodInfo> methodInfos)
		{
			// Use BindingFlags.DeclaredOnly to prevent including duplicate fields from base/derived classes
			bindingFlags |= BindingFlags.DeclaredOnly;

			do
			{
				methodInfos.AddRangeWhere(type.GetMethods(bindingFlags), item => !item.Exists(methodInfos));
				type = type.BaseType;
			}
			while (type != null);
		}

		/// <summary>
		/// C# doesn't give you base class private members even if you use BindingFlags.NonPublic.
		/// We have to manually search the base class(es) to find them.
		/// </summary>
		public static void GetAllFieldsWithAttribute(this Type type, BindingFlags bindingFlags, Type attributeType, List<FieldInfo> fieldInfos)
		{
			// Use BindingFlags.DeclaredOnly to prevent including duplicate fields from base/derived classes
			bindingFlags |= BindingFlags.DeclaredOnly;

			do
			{
				fieldInfos.AddRangeWhere(type.GetFields(bindingFlags), item => item.IsDefined(attributeType));
				type = type.BaseType;
			}
			while (type != null);
		}

		/// <summary>
		/// C# doesn't give you base class private members even if you use BindingFlags.NonPublic.
		/// We have to manually search the base class(es) to find them.
		/// </summary>
		public static void GetAllPropertiesWithAttribute(this Type type, BindingFlags bindingFlags, Type attributeType, List<PropertyInfo> propertyInfos)
		{
			// Use BindingFlags.DeclaredOnly to prevent including duplicate fields from base/derived classes
			bindingFlags |= BindingFlags.DeclaredOnly;

			do
			{
				propertyInfos.AddRangeWhere(type.GetProperties(bindingFlags), item => item.IsDefined(attributeType) && !item.Exists(propertyInfos));
				type = type.BaseType;
			}
			while (type != null);
		}

		/// <summary>
		/// C# doesn't give you base class private members even if you use BindingFlags.NonPublic.
		/// We have to manually search the base class(es) to find them.
		/// </summary>
		public static void GetAllMethodsWithAttribute(this Type type, BindingFlags bindingFlags, Type attributeType, List<MethodInfo> methodInfos)
		{
			// Use BindingFlags.DeclaredOnly to prevent including duplicate fields from base/derived classes
			bindingFlags |= BindingFlags.DeclaredOnly;

			do
			{
				methodInfos.AddRangeWhere(type.GetMethods(bindingFlags), item => item.IsDefined(attributeType) && !item.Exists(methodInfos));
				type = type.BaseType;
			}
			while (type != null);
		}

		public static bool HasDefaultConstructor(this Type type)
		{
			return type.IsValueType || type.GetConstructor(Type.EmptyTypes) != null;
		}

		private static bool Exists(this MethodInfo method, List<MethodInfo> infos)
		{
			MethodInfo methodBase = method.GetBaseDefinition();
			foreach (MethodInfo item in infos)
			{
				MethodInfo itemBase = item.GetBaseDefinition();
				if (method == item || method == itemBase || methodBase == itemBase)
				{
					return true;
				}
			}
			return false;
		}

		private static bool Exists(this PropertyInfo property, List<PropertyInfo> infos)
		{
			MethodInfo? propertyGetterBase = property.GetMethod?.GetBaseDefinition();
			MethodInfo? propertySetterBase = property.SetMethod?.GetBaseDefinition();
			foreach (PropertyInfo item in infos)
			{
				MethodInfo? itemGetterBase = item.GetMethod?.GetBaseDefinition();
				MethodInfo? itemSetterBase = item.SetMethod?.GetBaseDefinition();
				if (property == item
					|| itemGetterBase != null && (itemGetterBase == property.GetMethod || itemGetterBase == propertyGetterBase)
					|| itemSetterBase != null && (itemSetterBase == property.SetMethod || itemSetterBase == propertySetterBase))
				{
					return true;
				}
			}
			return false;
		}
	}
}