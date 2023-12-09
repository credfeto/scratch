using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using NonBlocking;

namespace Implementations;

public static class EnumHelpers
{
    private static readonly ConcurrentDictionary<Enum, string> CachedNames = new();

    public static string GetNameReflection<T>(this T value)
        where T : Enum
    {
        return Enum.GetName(value.GetType(), value: value) ?? InvalidName(value);
    }

    [DoesNotReturn]
    private static string InvalidName<T>(T value)
        where T : Enum
    {
        int iValue = Convert.ToInt32(value: value, provider: CultureInfo.InvariantCulture);

        return InvalidName(iValue);
    }

    [DoesNotReturn]
    private static string InvalidName(int value)
    {
        throw new InvalidOperationException(message: $"Unable to get name for {value}");
    }

    public static string GetName<T>(this T value)
        where T : Enum
    {
        return CachedNames.TryGetValue(key: value, out string? name)
            ? name
            : CachedNames.GetOrAdd(key: value, value.GetNameReflection());
    }
}