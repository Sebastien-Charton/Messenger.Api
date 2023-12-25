using System.Reflection;

namespace Messenger.Api.Domain.Extensions;

public static class TypeExtension
{
    public static T[] GetAllPublicConstantValues<T>(this Type type)
    {
        return type
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(T))
            .Select(x => (T)x.GetRawConstantValue()!)
            .ToArray();
    }
}
