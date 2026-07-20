using System;
using System.Reflection;

namespace Mauren.Net.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ApiServiceAttribute : Attribute
    {
        public required String Name { get; set; }
    }

    public static class ApiServiceAttributeExtensions
    {
        public static String GetKey(this Type type)
        {
            ApiServiceAttribute attribute = type.GetCustomAttribute<ApiServiceAttribute>()
                ?? throw new CustomAttributeFormatException($"{type.Name} is missing the {nameof(ApiServiceAttribute)}");
            return attribute.Name;
        }
    }
}
