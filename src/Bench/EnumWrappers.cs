using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Implementations;

namespace Bench;

internal static class EnumWrappers
{
    [SuppressMessage(category: "ReSharper", checkId: "InvokeAsExtensionMethod", Justification = "This is a benchmark.")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetNameReflectionCached<T>(this T value)
        where T : Enum
    {
        // ReSharper disable once InvokeAsExtensionMethod
        return EnumHelpers.GetName(value);
    }
}