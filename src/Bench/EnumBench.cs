using BenchmarkDotNet.Attributes;
using Implementations;
using Models;

namespace Bench;

[SimpleJob]
[MemoryDiagnoser(false)]
public abstract class EnumBench : BenchBase
{
    [Benchmark]
    public void GetNameReflection()
    {
        this.Test(ExampleEnumValues.ONE.GetNameReflection());
    }

    [Benchmark]
    public void GetNameCachedReflection()
    {
        this.Test(ExampleEnumValues.ONE.GetNameCachedReflection());
    }
}