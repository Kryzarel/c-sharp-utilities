using System;
using System.Collections.Generic;
using System.Reflection;

namespace Kryz.Utils
{
	public static class TypeExtensions
	{
		/// <summary>
		/// C# doesn't give you base class private members even if you use BindingFlags.NonPublic.
		/// We have to manually search the base class(es) to find them.
		/// </summary>
		public static FieldInfo? GetFieldInSubclasses(this Type type, string name, BindingFlags bindingFlags)
		{
			// Use BindingFlags.DeclaredOnly to prevent including duplicate members from base/derived classes
			bindingFlags |= BindingFlags.DeclaredOnly;

			FieldInfo? field = null;
			for (Type t = type; t != null && field == null; t = t.BaseType)
			{
				field = t.GetField(name, bindingFlags);
			}
			return field;
		}

		/// <summary>
		/// C# doesn't give you base class private members even if you use BindingFlags.NonPublic.
		/// We have to manually search the base class(es) to find them.
		/// </summary>
		public static void GetAllFields(this Type type, BindingFlags bindingFlags, IList<FieldInfo> fieldInfos)
		{
			// Use BindingFlags.DeclaredOnly to prevent including duplicate members from base/derived classes
			bindingFlags |= BindingFlags.DeclaredOnly;

			for (Type t = type; t != null; t = t.BaseType)
			{
				fieldInfos.AddRangeNonAlloc((IReadOnlyList<FieldInfo>)t.GetFields(bindingFlags));
			}
		}

		/// <summary>
		/// C# doesn't give you base class private members even if you use BindingFlags.NonPublic.
		/// We have to manually search the base class(es) to find them.
		/// </summary>
		public static void GetAllProperties(this Type type, BindingFlags bindingFlags, IList<PropertyInfo> propertyInfos)
		{
			// Use BindingFlags.DeclaredOnly to prevent including duplicate members from base/derived classes
			bindingFlags |= BindingFlags.DeclaredOnly;

			for (Type t = type; t != null; t = t.BaseType)
			{
				foreach (PropertyInfo item in t.GetProperties(bindingFlags))
				{
					if (!item.IsDuplicate(propertyInfos))
					{
						propertyInfos.Add(item);
					}
				}
			}
		}

		/// <summary>
		/// C# doesn't give you base class private members even if you use BindingFlags.NonPublic.
		/// We have to manually search the base class(es) to find them.
		/// </summary>
		public static void GetAllMethods(this Type type, BindingFlags bindingFlags, IList<MethodInfo> methodInfos)
		{
			// Use BindingFlags.DeclaredOnly to prevent including duplicate members from base/derived classes
			bindingFlags |= BindingFlags.DeclaredOnly;

			for (Type t = type; t != null; t = t.BaseType)
			{
				foreach (MethodInfo item in t.GetMethods(bindingFlags))
				{
					if (!item.IsDuplicate(methodInfos))
					{
						methodInfos.Add(item);
					}
				}
			}
		}

		/// <summary>
		/// C# doesn't give you base class private members even if you use BindingFlags.NonPublic.
		/// We have to manually search the base class(es) to find them.
		/// </summary>
		public static void GetAllFieldsWithAttribute(this Type type, BindingFlags bindingFlags, Type attributeType, IList<FieldInfo> fieldInfos)
		{
			// Use BindingFlags.DeclaredOnly to prevent including duplicate members from base/derived classes
			bindingFlags |= BindingFlags.DeclaredOnly;

			for (Type t = type; t != null; t = t.BaseType)
			{
				foreach (FieldInfo item in t.GetFields(bindingFlags))
				{
					if (item.IsDefined(attributeType))
					{
						fieldInfos.Add(item);
					}
				}
			}
		}

		/// <summary>
		/// C# doesn't give you base class private members even if you use BindingFlags.NonPublic.
		/// We have to manually search the base class(es) to find them.
		/// </summary>
		public static void GetAllPropertiesWithAttribute(this Type type, BindingFlags bindingFlags, Type attributeType, IList<PropertyInfo> propertyInfos)
		{
			// Use BindingFlags.DeclaredOnly to prevent including duplicate members from base/derived classes
			bindingFlags |= BindingFlags.DeclaredOnly;

			for (Type t = type; t != null; t = t.BaseType)
			{
				foreach (PropertyInfo item in t.GetProperties(bindingFlags))
				{
					if (item.IsDefined(attributeType) && !item.IsDuplicate(propertyInfos))
					{
						propertyInfos.Add(item);
					}
				}
			}
		}

		/// <summary>
		/// C# doesn't give you base class private members even if you use BindingFlags.NonPublic.
		/// We have to manually search the base class(es) to find them.
		/// </summary>
		public static void GetAllMethodsWithAttribute(this Type type, BindingFlags bindingFlags, Type attributeType, IList<MethodInfo> methodInfos)
		{
			// Use BindingFlags.DeclaredOnly to prevent including duplicate members from base/derived classes
			bindingFlags |= BindingFlags.DeclaredOnly;

			for (Type t = type; t != null; t = t.BaseType)
			{
				foreach (MethodInfo item in t.GetMethods(bindingFlags))
				{
					if (item.IsDefined(attributeType) && !item.IsDuplicate(methodInfos))
					{
						methodInfos.Add(item);
					}
				}
			}
		}

		public static bool HasDefaultConstructor(this Type type)
		{
			return type.IsValueType || type.GetConstructor(Type.EmptyTypes) != null;
		}

		public static Type GetMemberType(this MemberInfo memberInfo)
		{
			return memberInfo switch
			{
				FieldInfo fieldInfo => fieldInfo.FieldType,
				PropertyInfo propertyInfo => propertyInfo.PropertyType,
				MethodInfo methodInfo => methodInfo.ReturnType,
				EventInfo eventInfo => eventInfo.EventHandlerType,
				_ => throw new NotImplementedException(),
			};
		}

		private static bool IsDuplicate(this MethodInfo method, IList<MethodInfo> infos)
		{
			MethodInfo methodBase = method.GetBaseDefinition();
			for (int i = 0; i < infos.Count; i++)
			{
				MethodInfo item = infos[i];
				MethodInfo itemBase = item.GetBaseDefinition();
				if (method == item || method == itemBase || methodBase == itemBase)
				{
					return true;
				}
			}
			return false;
		}

		private static bool IsDuplicate(this PropertyInfo property, IList<PropertyInfo> infos)
		{
			MethodInfo? propertyGetterBase = property.GetMethod?.GetBaseDefinition();
			MethodInfo? propertySetterBase = property.SetMethod?.GetBaseDefinition();
			for (int i = 0; i < infos.Count; i++)
			{
				PropertyInfo item = infos[i];
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