using System;
using System.Diagnostics.CodeAnalysis;
using BenchmarkDotNet.Attributes;
using Implementations.InlineStructOptimisations;

namespace Bench.Benchmarks;

[SimpleJob]
[MemoryDiagnoser(false)]
[SuppressMessage(category: "", checkId: "SCS0005", Justification = "Needed for predictability")]
[SuppressMessage(category: "", checkId: "CA5394", Justification = "Needed for predictability")]
public abstract class InlineArrayBench : BenchBase
{
    private const int SIZE = 32;
    private const int ITERATIONS = 1000;

    private readonly Random _randomSource = new(1234);

    [Benchmark]
    public void UsingReadOnlyMemory()
    {
        for (int iteration = 0; iteration < ITERATIONS; ++iteration)
        {
            byte[] item = new byte[SIZE];

            this._randomSource.NextBytes(item);

            this.Test(new StorageTest<ReadOnlyMemory<byte>>(item));
        }
    }

    [Benchmark]
    public void UsingInlineArray()
    {
        Span<byte> source = stackalloc byte[SIZE];

        for (int iteration = 0; iteration < ITERATIONS; ++iteration)
        {
            this._randomSource.NextBytes(source);

            KeccakSizedArray item = new(source);

            this.Test(new StorageTest<KeccakSizedArray>(item));
        }
    }
}
