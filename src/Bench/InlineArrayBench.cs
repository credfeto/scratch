using System;
using System.Diagnostics.CodeAnalysis;
using BenchmarkDotNet.Attributes;
using Implementations.InlineStructOptimisations;

namespace Bench;

[SimpleJob]
[MemoryDiagnoser(false)]
[SuppressMessage(category: "", checkId: "SCS0005", Justification = "Needed for predictability")]
[SuppressMessage(category: "", checkId: "CA5394", Justification = "Needed for predictability")]
[SuppressMessage(category: "", checkId: "CS1503", Justification = "Needed for predictability")]
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

            for (int i = 0; i < SIZE; ++i)
            {
                item[i] = (byte)this._randomSource.Next(minValue: 0, maxValue: 255);
            }

            this.Test(new StorageTest<ReadOnlyMemory<byte>>(item));
        }
    }

    [Benchmark]
    public void UsingInlineArray()
    {
        Span<byte> source = stackalloc byte[SIZE];

        for (int iteration = 0; iteration < ITERATIONS; ++iteration)
        {
            for (int i = 0; i < SIZE; ++i)
            {
                source[i] = (byte)this._randomSource.Next(minValue: 0, maxValue: 255);
            }

            KeccakSizedArray item = new(source);

            this.Test(new StorageTest<KeccakSizedArray>(item));
        }
    }
}