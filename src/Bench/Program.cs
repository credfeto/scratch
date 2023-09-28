#define INLINE_ARRAY
using System.Diagnostics;
using BenchmarkDotNet.Running;

namespace Bench;

public static class Program
{
    private static void Main()
    {
        RunRegexBenchmarks();
        RunMemoryBenchmarks();
        RunInlineArrayBenchmarks();
    }

    [Conditional("REGEX")]
    private static void RunRegexBenchmarks()
    {
        BenchmarkRunner.Run<RegexBench>();
    }

    [Conditional("MEMORY")]
    private static void RunMemoryBenchmarks()
    {
        BenchmarkRunner.Run<JsonDeserialiserMemoryBench>();
    }

    [Conditional("INLINE_ARRAY")]
    private static void RunInlineArrayBenchmarks()
    {
        BenchmarkRunner.Run<InlineArrayBench>();
    }
}