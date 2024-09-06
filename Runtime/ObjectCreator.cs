using System;
using System.Runtime.Serialization;

namespace Kryz.SharpUtils
{
	public static class ObjectCreator
	{
		public static T Create<T>()
		{
			Type type = typeof(T);
			if (type.HasDefaultConstructor())
			{
				return Activator.CreateInstance<T>();
			}
			return (T)FormatterServices.GetUninitializedObject(type);
		}

		public static object Create(Type type)
		{
			if (type.HasDefaultConstructor())
			{
				return Activator.CreateInstance(type);
			}
			return FormatterServices.GetUninitializedObject(type);
		}
	}
}