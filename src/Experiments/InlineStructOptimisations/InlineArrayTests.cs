using System;
using FunFair.Test.Common;
using Xunit;

namespace Experiments.InlineStructOptimisations;

public sealed class InlineArrayTests : TestBase
{
    [Fact]
    public static void UsingReadOnlyMemory()
    {
        StorageTest<ReadOnlyMemory<byte>> test = new(new byte[32]);

        DoIt(test);
    }

    [Fact]
    public static void UsingInlineArray()
    {
        StorageTest<KeccakSizedArray> test = new(new());
        DoIt(test);
    }

    private static void DoIt<T>(in StorageTest<T> test)
        where T : struct
    {
        UnusedVariable(test);
    }
}