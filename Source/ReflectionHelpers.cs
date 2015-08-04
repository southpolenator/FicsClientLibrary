namespace Internet.Chess.Server
{
    using System;
    using System.Linq;
    using System.Reflection;

    public static class ReflectionHelpers
    {
        public static Type[] GetInterfaces(this Type type)
        {
            return type.GetTypeInfo().ImplementedInterfaces.ToArray();
        }

        public static PropertyInfo[] GetProperties(this Type type)
        {
            return type.GetTypeInfo().DeclaredProperties.ToArray();
        }

        public static PropertyInfo GetProperty(this Type type, string name)
        {
            return type.GetTypeInfo().GetDeclaredProperty(name);
        }

        public static T GetSingleAttribute<T>(this Enum enumValue) where T : Attribute
        {
            return enumValue.GetType().GetTypeInfo().GetDeclaredField(enumValue.ToString()).GetCustomAttribute<T>();
        }

        public static T GetSingleAttribute<T>(this PropertyInfo property) where T : Attribute
        {
            return property.GetCustomAttribute<T>();
        }
    }
}
