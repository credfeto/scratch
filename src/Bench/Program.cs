#define MEMORY
using System.Diagnostics;
using BenchmarkDotNet.Running;

namespace Bench;

public static class Program
{
    private static void Main()
    {
        RunRegexBenchmarks();
        RunMemoryBenchmarks();
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
}