using System;
using NonBlocking;

namespace Implementations;

public static class EnumHelpers
{
    private static readonly ConcurrentDictionary<Enum, string> CachedNames = new();

    public static string GetNameReflection<T>(this T value)
        where T : Enum
    {
        return Enum.GetName(value.GetType(), value: value) ?? UnknownName(value);
    }

    public static string GetNameCachedReflection<T>(this T value)
        where T : Enum
    {
        if (CachedNames.TryGetValue(key: value, out string? name))
        {
            return name;
        }

        return CachedNames.GetOrAdd(key: value, value.GetNameReflection());
    }

    private static string UnknownName(Enum value)
    {
        return "??" + value;
    }
}