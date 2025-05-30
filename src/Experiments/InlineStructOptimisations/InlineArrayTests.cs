using System;
using System.Diagnostics;
using FunFair.Test.Common;
using Implementations.InlineStructOptimisations;
using Xunit;

namespace Experiments.InlineStructOptimisations;

public sealed class InlineArrayTests : LoggingTestBase
{
    public InlineArrayTests(ITestOutputHelper output)
        : base(output) { }

    [Fact]
    public void UsingReadOnlyMemory()
    {
        byte[] source = new byte[32];

        for (int i = 0; i < 32; ++i)
        {
            source[i] = (byte)i;
        }

        StorageTest<ReadOnlyMemory<byte>> test = new(source);

        DoIt(test);

        this.Dump(test);
    }

    [Fact]
    public void UsingInlineArray()
    {
        KeccakSizedArray source = CreateInlineArray();

        StorageTest<KeccakSizedArray> test = CreateInlineStorage(source);

        DoIt(test);

        this.Dump(test);
    }

    [Fact]
    public void UsingInlineArrayInClass()
    {
        KeccakSizedArray source = CreateInlineArray();

        StorageTest<KeccakSizedArray> test = CreateInlineStorage(source);

        St s = new(test);

        this.Dump(s.Test);
    }

    private static StorageTest<KeccakSizedArray> CreateInlineStorage(KeccakSizedArray source)
    {
        StorageTest<KeccakSizedArray> test = new(source);

        return test;
    }

    private static KeccakSizedArray CreateInlineArray()
    {
        Span<byte> source = stackalloc byte[KeccakSizedArray.Length];

        for (int i = 0; i < 32; ++i)
        {
            source[i] = (byte)(32 - i);
        }

        return new(source);
    }

    private void Dump(in StorageTest<KeccakSizedArray> test)
    {
        for (int i = 0; i < 32; ++i)
        {
            this.Output.WriteLine($"{i}: {test.Bytes[i]}");
        }
    }

    private void Dump(in StorageTest<ReadOnlyMemory<byte>> test)
    {
        this.Dump(test.Bytes.Span);
    }

    private void Dump(in ReadOnlySpan<byte> sp)
    {
        for (int i = 0; i < 32; ++i)
        {
            this.Output.WriteLine($"{i}: {sp[i]}");
        }
    }

    private static void DoIt<T>(in StorageTest<T> test)
        where T : struct
    {
        UnusedVariable(test);
    }

    [DebuggerDisplay("{Test}")]
    private sealed record St(StorageTest<KeccakSizedArray> Test);
}
