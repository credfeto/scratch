using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace Bench;

public abstract class BenchBase
{
    [SuppressMessage(category: "Microsoft.Performance", checkId: "CA1822:Mark methods static", Justification = "Needed for BenchmarkDotNet")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedParameter.Global", Justification = "Simplifies benchmarks")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Pure]
    protected void Test<T>(T value)
    {
        // Doesn't do anything
    }
}